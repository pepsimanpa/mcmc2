#!/usr/bin/env python3
"""Apply source-backed MDV/EMDW Binding fixes without reformatting XML.

The script performs exact, scope-limited replacements so the original Binding
format is preserved. It aborts when the current XML no longer matches the
expected structure.

Usage:
    python ProtocolXml/Tools/ApplyMdvEmdwBindingPatch.py --check
    python ProtocolXml/Tools/ApplyMdvEmdwBindingPatch.py --apply
"""

from __future__ import annotations

import argparse
import re
import sys
import xml.etree.ElementTree as ET
from pathlib import Path

REPO_ROOT = Path(__file__).resolve().parents[2]
MDV = REPO_ROOT / "ProtocolXml" / "MDV" / "MdvTcpBinding.xml"
EMDW = REPO_ROOT / "ProtocolXml" / "EMDW" / "EmdwTcpBinding.xml"


class PatchError(RuntimeError):
    pass


def replace_exact(text: str, old: str, new: str, expected: int, label: str) -> str:
    count = text.count(old)
    if count != expected:
        raise PatchError(f"{label}: expected {expected} occurrence(s), found {count}")
    return text.replace(old, new)


def replace_in_tag_block(
    text: str,
    tag: str,
    semantic_id: str,
    old: str,
    new: str,
    expected: int,
    label: str,
) -> str:
    pattern = re.compile(
        rf'(<{tag}\s+semantic_id="{re.escape(semantic_id)}"\b[\s\S]*?</{tag}>)'
    )
    matches = list(pattern.finditer(text))
    if len(matches) != 1:
        raise PatchError(f"{label}: expected exactly one {tag} block, found {len(matches)}")
    match = matches[0]
    block = match.group(1)
    count = block.count(old)
    if count != expected:
        raise PatchError(f"{label}: expected {expected} occurrence(s) in block, found {count}")
    new_block = block.replace(old, new)
    return text[: match.start(1)] + new_block + text[match.end(1) :]


def patch_reply(
    text: str,
    reply_id: str,
    platform_name: str,
    reason_converter: str,
) -> str:
    label = f"{platform_name}:{reply_id}"
    text = replace_in_tag_block(
        text,
        "Reply",
        reply_id,
        '<Field name="result" cdm="Command.Result"/>',
        '<Field name="result" cdm="Command.Result" converter="CommandResultCodeToState"/>',
        1,
        f"{label}:result converter",
    )
    text = replace_in_tag_block(
        text,
        "Reply",
        reply_id,
        '<Field name="reasonCode" cdm="Command.Result.ReasonCode" converter="UInt16LE"/>',
        f'<Field name="reasonCode" cdm="Command.Result.ReasonCode" converter="{reason_converter}"/>',
        1,
        f"{label}:reason converter",
    )
    old_comment = (
        "result=0은 요청 검증 완료 및 명령 수락을 의미하며, "
        "실제 진행/완료는 관련 상태정보로 확인한다."
    )
    new_comment = (
        "result는 원 ICD의 명령 처리 결과이며 0=성공, 1=실패이다. "
        "실제 임무 진행/완료는 관련 상태정보로 확인한다."
    )
    text = replace_in_tag_block(
        text,
        "Reply",
        reply_id,
        old_comment,
        new_comment,
        1,
        f"{label}:result comment",
    )
    return text


def patch_mdv(text: str) -> str:
    text = replace_exact(
        text,
        "T_HEARTBEAT는 현 XSD의 Binding-only 표현 부재로 기존 MonitorBinding을 임시 유지한다.",
        "T_HEARTBEAT의 물리 양방향 송수신은 유지하며 Semantic Monitor 방향성은 별도 TBD로 관리한다.",
        1,
        "MDV:heartbeat header comment",
    )
    text = replace_in_tag_block(
        text,
        "MonitorBinding",
        "transmitHeartbeat",
        '<Field name="headerReserved"/>',
        '<FixedField name="headerReserved" value="0"/>',
        1,
        "MDV:transmitHeartbeat reserved",
    )
    text = replace_in_tag_block(
        text,
        "MonitorBinding",
        "missionExecutionStatus",
        '<Field name="currentWaypointIndex" cdm="MissionPlan.CurrentWaypointIndex" converter="UInt16LE"/>',
        '<Field name="currentWaypointIndex" cdm="MissionPlan.CurrentWaypointIndex" converter="UInt16LECurrentWaypointIndexOrNone"/>',
        1,
        "MDV:currentWaypointIndex sentinel",
    )
    text = replace_in_tag_block(
        text,
        "ControlBinding",
        "transferMissionPlan",
        '<Field name="action" cdm="MissionPlan.Waypoint.Action"/>',
        '<Field name="action" cdm="MissionPlan.Waypoint.Action" converter="MissionWaypointActionToCode"/>',
        1,
        "MDV:waypoint action",
    )
    text = replace_in_tag_block(
        text,
        "ControlBinding",
        "transferMissionPlan",
        '<Field name="missionType" cdm="Mission.Type"/>',
        '<Field name="missionType" cdm="Mission.Type" converter="MissionTypeToCode"/>',
        1,
        "MDV:mission type",
    )
    for control_id in ("immediateEmergencyStop", "emergencyStopAndStandby"):
        text = replace_in_tag_block(
            text,
            "ControlBinding",
            control_id,
            '<Field name="emergencyStopReason" cdm="Platform.EmergencyStop.Reason"/>',
            '<Field name="emergencyStopReason" cdm="Platform.EmergencyStop.Reason" converter="EmergencyStopReasonToCode"/>',
            1,
            f"MDV:{control_id}:emergency reason",
        )
    for reply_id in (
        "stopMissionReply",
        "startMissionReply",
        "pauseMissionReply",
        "resumeMissionReply",
        "setLegacyModeReply",
        "setAutonomousModeReply",
        "transferMissionPlanReply",
        "immediateEmergencyStopReply",
        "emergencyStopAndStandbyReply",
    ):
        text = patch_reply(text, reply_id, "MDV", "UInt16LEToMdvCommandReasonState")
    return text


def patch_emdw(text: str) -> str:
    text = replace_exact(
        text,
        "T_HEARTBEAT는 현 XSD의 Binding-only 표현 부재로 기존 MonitorBinding을 임시 유지한다.",
        "T_HEARTBEAT의 물리 양방향 송수신은 유지하며 Semantic Monitor 방향성은 별도 TBD로 관리한다.",
        1,
        "EMDW:heartbeat header comment",
    )
    text = replace_in_tag_block(
        text,
        "MonitorBinding",
        "transmitHeartbeat",
        '<Field name="headerReserved"/>',
        '<FixedField name="headerReserved" value="0"/>',
        1,
        "EMDW:transmitHeartbeat reserved",
    )
    text = replace_in_tag_block(
        text,
        "MonitorBinding",
        "missionExecutionStatus",
        '<Field name="currentWaypointIndex" cdm="MissionPlan.CurrentWaypointIndex" converter="UInt16LE"/>',
        '<Field name="currentWaypointIndex" cdm="MissionPlan.CurrentWaypointIndex" converter="UInt16LECurrentWaypointIndexOrNone"/>',
        1,
        "EMDW:currentWaypointIndex sentinel",
    )
    text = replace_in_tag_block(
        text,
        "ControlBinding",
        "transferMissionPlan",
        '<Field name="action" cdm="MissionPlan.Waypoint.Action"/>',
        '<Field name="action" cdm="MissionPlan.Waypoint.Action" converter="MissionWaypointActionToCode"/>',
        1,
        "EMDW:waypoint action",
    )
    text = replace_in_tag_block(
        text,
        "ControlBinding",
        "transferMissionPlan",
        '<Field name="missionType" cdm="Mission.Type"/>',
        '<Field name="missionType" cdm="Mission.Type" converter="MissionTypeToCode"/>',
        1,
        "EMDW:mission type",
    )
    for control_id in ("immediateEmergencyStop", "emergencyStopAndStandby"):
        text = replace_in_tag_block(
            text,
            "ControlBinding",
            control_id,
            '<Field name="emergencyStopReason" cdm="Platform.EmergencyStop.Reason"/>',
            '<Field name="emergencyStopReason" cdm="Platform.EmergencyStop.Reason" converter="EmergencyStopReasonToCode"/>',
            1,
            f"EMDW:{control_id}:emergency reason",
        )
    for reply_id in (
        "stopMissionReply",
        "startMissionReply",
        "pauseMissionReply",
        "resumeMissionReply",
        "setTerminalControlModeReply",
        "setIntegratedControlModeReply",
        "transferMissionPlanReply",
        "immediateEmergencyStopReply",
        "emergencyStopAndStandbyReply",
        "cancelDisposalReply",
        "prepareDisposalReply",
        "executeDisposalReply",
    ):
        text = patch_reply(text, reply_id, "EMDW", "UInt16LEToEmdwCommandReasonState")
    return text


def parse_xml_text(text: str, label: str) -> None:
    try:
        ET.fromstring(text)
    except ET.ParseError as exc:
        raise PatchError(f"{label}: XML parse failed: {exc}") from exc


def validate_patched(mdv: str, emdw: str) -> None:
    checks = (
        (mdv.count('converter="CommandResultCodeToState"'), 9, "MDV result converter count"),
        (mdv.count('converter="UInt16LEToMdvCommandReasonState"'), 9, "MDV reason converter count"),
        (emdw.count('converter="CommandResultCodeToState"'), 12, "EMDW result converter count"),
        (emdw.count('converter="UInt16LEToEmdwCommandReasonState"'), 12, "EMDW reason converter count"),
        (mdv.count('converter="MissionWaypointActionToCode"'), 1, "MDV waypoint action converter"),
        (emdw.count('converter="MissionWaypointActionToCode"'), 1, "EMDW waypoint action converter"),
        (mdv.count('converter="MissionTypeToCode"'), 1, "MDV mission type converter"),
        (emdw.count('converter="MissionTypeToCode"'), 1, "EMDW mission type converter"),
        (mdv.count('converter="EmergencyStopReasonToCode"'), 2, "MDV emergency reason converter"),
        (emdw.count('converter="EmergencyStopReasonToCode"'), 2, "EMDW emergency reason converter"),
        (mdv.count('converter="UInt16LECurrentWaypointIndexOrNone"'), 1, "MDV waypoint sentinel converter"),
        (emdw.count('converter="UInt16LECurrentWaypointIndexOrNone"'), 1, "EMDW waypoint sentinel converter"),
    )
    for actual, expected, label in checks:
        if actual != expected:
            raise PatchError(f"{label}: expected {expected}, found {actual}")

    old_interpretation = "result=0은 요청 검증 완료 및 명령 수락"
    if old_interpretation in mdv or old_interpretation in emdw:
        raise PatchError("old result=0 'command accepted' interpretation still remains")

    for label, text in (("MDV", mdv), ("EMDW", emdw)):
        parse_xml_text(text, label)


def main() -> int:
    parser = argparse.ArgumentParser()
    mode = parser.add_mutually_exclusive_group(required=True)
    mode.add_argument("--check", action="store_true", help="verify the patch can be applied cleanly")
    mode.add_argument("--apply", action="store_true", help="apply the patch in place")
    args = parser.parse_args()

    try:
        mdv_original = MDV.read_text(encoding="utf-8")
        emdw_original = EMDW.read_text(encoding="utf-8")
        mdv_patched = patch_mdv(mdv_original)
        emdw_patched = patch_emdw(emdw_original)
        validate_patched(mdv_patched, emdw_patched)

        if args.apply:
            MDV.write_text(mdv_patched, encoding="utf-8", newline="")
            EMDW.write_text(emdw_patched, encoding="utf-8", newline="")
            print("Applied MDV/EMDW Binding patch successfully.")
        else:
            print("MDV/EMDW Binding patch preflight passed; no files changed.")
        return 0
    except (OSError, PatchError) as exc:
        print(f"ERROR: {exc}", file=sys.stderr)
        return 1


if __name__ == "__main__":
    raise SystemExit(main())
