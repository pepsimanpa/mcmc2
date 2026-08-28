# 기뢰전 지휘통제 설계 — Semantic / Binding / USV 작업 HANDOFF

- 작성일: **2026-08-28**
- 작성 시각 기준: **2026-08-28 00:30 KST**
- 목적: 새 ChatGPT 채팅에서 기존 설계 의도, 확정 규칙, Git 상태, 완료 장치, 미해결 이슈를 다시 설명하지 않고 바로 작업을 이어가기 위한 인계 문서
- Git 저장소: `pepsimanpa/mcmc2`
- 기본 브랜치: `main`
- 현재 USV 작업 브랜치: `feature/usv-semantic-binding-audit`
- 작성 직전 작업 브랜치 HEAD: `431e2f2016189f174da7efbe03a2b8451a423d1e`
- `main` HEAD: `f131851f3f59ad9da986eb2cc2b33f6beed5adc0`
- USV 작업 브랜치 상태: 작성 직전 `main` 대비 **27 commits ahead / 0 behind**
- 현재 집중 범위: **USV 장치별 Semantic / Binding 전수 정리**

> 새 채팅에서는 이 문서를 먼저 읽고, 실제 수정 전 반드시 GitHub의 최신 브랜치 HEAD와 대상 XML/XSD/원 CSCI·CSV를 다시 확인한다. 문서와 원문이 충돌하면 **원 ICD/CSCI/원작자 자료 → 통합 설계 규칙 문서 → 현재 Git XML → 본 HANDOFF** 순서로 재검증한다.

---

## 1. 공통 설계 기준

### 근거 우선순위
1. 원 ICD / CSCI / 원작자 XML / 원작자 XSD의 직접 명시
2. `ProtocolXml/Docs/UMS_Semantic_Binding_Design_Rules_20260826.md`
3. 현재 Git의 Semantic / Binding / Specification
4. 추론

추론은 원문 사실처럼 쓰지 않는다. 애매한 사항은 XML 주석 또는 장치별 `*_OpenIssues.md`에 남긴다.

### 계층별 책임
- **Semantic**: Control 의미, Target, 사용자가 입력/선택하는 Parameter, 단위/범위/해상도, Reply 논리결과, Monitor, SensorProduct, 사용자가 이해할 상태 의미.
- **Binding**: Protocol/Channel/Topic/Type, Field/FixedField/DerivedField/PackedField/BitMember/ArrayField, `dataType`, `scale`, `length`, `format`, `ValueMap`, `Sentinel/Normal`, `sourceField`, Reply의 실제 wire 구조.
- **OM/실행 로직**: 값 생성/증가, 업무 규칙 판단, checksum 생성/검증. Binding 안에 실행 함수 이름을 두지 않는다.
- **Specification**: Semantic+Binding을 묶고 `<DataEncoding>` 같은 시스템 공통 규칙을 제공한다.

### 선언형 Binding
- converter 의존 제거
- enum raw 숫자는 Semantic이 아니라 Binding `ValueMap`
- endian은 상위 Specification에서 선언
- `commandID`, timestamp, checksum, heartbeat sequence, 배열 count 등은 OM/Adapter가 준비
- 송신 checksum 생성 / 수신 checksum 검증은 OM/framing 책임

---

## 2. Control / Reply / Monitor / SensorProduct

- `Control`은 송신 방향이 아니라 원격으로 수행 가능한 기능이다.
- `Reply`는 해당 Control의 응답이다.
- `Monitor`는 주기/지속 감시 대상이며 수신 방향 자체를 의미하지 않는다.
- `SensorProduct`는 영상/파일/센서 스트림처럼 일반 Reply와 분리할 데이터를 표현한다.

USV DDS 공통 패턴:
```text
PBIT/IBIT Control
  ├─ CommandStatusReportType ACK
  └─ 별도 PBIT/IBIT Result Report
```

`CommandStatusReport`는 명령 처리상태이며 실제 물리 동작 완료/성공으로 확대 해석하지 않는다. 공용 상태는 `Executing / Pending / Failed / Reject / Canceled`이다.

---

## 3. XML 작업 스타일 — 반드시 유지

1. **기존 유효한 주석을 삭제하지 않는다.**
2. XML 전체 재직렬화/대규모 자동 포맷 변경을 피한다.
3. 필요한 부분만 최소 diff로 수정한다.
4. 의미가 바뀐 주석은 현재 확정 설계에 맞게 갱신한다.
5. 애매한 사항은 지우지 말고 주석/OpenIssues에 보존한다.
6. 원 CSCI의 Type 이름 오탈자는 DDS `typeName` 정합성을 위해 함부로 고치지 않는다.
7. 최종 CDM 명칭/계층 감사는 USV 장치 전체를 정리한 후 한 번에 수행한다.
8. OM 실제 구현/validator/Adapter 구현 여부는 Semantic/Binding 설계 범위 밖이다.

---

## 4. AUV — 완료 및 main 병합

AUV는 다음까지 완료되어 `main`에 병합되었다.

- RF-1 / RF-2 / RF-3 물리 구조 정리
- RF-2 80-byte 고정 구조 정상화
- RF-3 일반 Reply → SensorProduct 분리
- UCD 구조 정리
- COMMAND / ACK / INFO_NUM 정합성
- `MISSION_START`는 프로젝트 결정으로 INFORMATION3
- OperationMode: Semantic `광역탐색/정밀탐색`, Binding `1/2`
- `Platform.Identifier.Numeric = 1 또는 2`
- INFO_NUM: INFO1 `1→1/2→4`, INFO2 `1→2/2→5`, INFO3 `1→3/2→6`
- `SourceValueMap` 도입
- AUV converter 제거
- checksum OM 책임 분리
- bit 해석: 점검 `0=PASS/1=FAIL`, EOR `0=미발생/1=발생`, MODE `0=비활성/1=활성`, STAT bit0~4 `0=정상/1=비정상`, DATA_LOCK/SYNC_LOCK은 활성 flag
- XSD validation 및 구조 정합성 감사 완료

`main` merge commit: `f131851f3f59ad9da986eb2cc2b33f6beed5adc0`

---

## 5. USV 공통 작업 전략

장치별로 다음 원본을 함께 본다.
- `공용 식별자 규칙.csv`
- `공용 규칙.csv`
- `공용 구조체.csv`
- `원격통제장치 CSCI.csv`
- 대상 장치 CSCI

장치 하나를 가능한 한 한 번에:
1. 원 메시지/Topic/Type 수집
2. Semantic Control/Reply/Monitor/SensorProduct 대조
3. Binding 대조
4. 공용 Header/Destination/commandID 적용
5. converter 제거
6. 원 CSCI 확정 primitive `dataType` 보강
7. Semantic raw enum → 논리값, Binding ValueMap
8. Reply payload → Semantic Results
9. XSD validation
10. ID/cross-reference 감사
11. 주석 보존 검증
12. 애매한 것은 `*_OpenIssues.md`

사용자 요청: SSS에서 이미 해결한 공통 이슈는 다음 장치마다 다시 설명하지 않고 자동 적용하며, **장치 고유 이슈만 최종 결과에 보고**한다.

---

## 6. SideScanSonar(SSS) — 구조 완료

경로: `ProtocolXml/USV/SideScanSonar/`

주요 파일:
- `SideScanSonarSemantic.xml`
- `SideScanSonarBinding.xml`
- `SideScanSonar_OpenIssues.md`

### 완료
- SSS 로컬 구형 XSD 대신 `ProtocolXml/XSD` 공통 XSD 사용
- 로컬 XSD 파일 자체는 외부 의존 가능성 때문에 아직 삭제하지 않음
- converter 80건 → **0건**
- `USVMessageBase`, `DestinationType` 조립은 OM/Adapter 책임
- SSS destination: `0x04 / 0x01 / 0x00`
- commandID: 일반 sequence 33, `LastReceivedCommandID` 계승 5
- primitive dataType 340개 보강: UInt8 211 / UInt16 54 / Float32 73 / Float64 2
- primitive dataType 누락 0; composite `usvHeader`, `destination`만 의도적으로 dataType 없음
- raw enum → Semantic 논리값 + Binding ValueMap
- Control `38↔38`
- Reply `47↔47`
- PBIT: **한 개 결과 Reply + GroupResult 내부 6개**
- IBIT: **한 개 결과 Reply + GroupResult 내부 62개**
- SensorProduct `5↔5`: SSS 압축 / GapFiller 압축 / 병합 압축 / ObjectDetection2D / 진회수 카메라 RTP

### SSS 핵심 잔여 TBD
상세는 `SideScanSonar_OpenIssues.md`에 보존.
- `LastReceivedCommandID` 동시 명령 correlation
- commandID 없는 결과 Report correlation
- powerStatus 5개의 0/1 의미
- IBIT detail의 이상 flag/장착유무 flag 혼재
- dotted path Adapter 정식 규칙 여부
- 로컬 구형 XSD 삭제 여부
- 최종 CDM 감사
- `TVG.Mode`, `SoftwareGain.Mode`, `LowPassFilter.Mode` 논리명
- BackStop 결과 raw 의미
- 조건부 Parameter 표현 방식

SSS는 **구조 완료 / 비차단 TBD 상태**로 닫음.

---

## 7. CentralControl — 구조 완료

경로: `ProtocolXml/USV/CentralControl/`

주요 파일:
- `CentralControlSemantic.xml`
- `CentralControlBinding.xml`
- `CentralControl_OpenIssues.md`

최종 본 작업 commit: `37f26127...` — `Finalize CentralControl semantic binding audit`
후속 주석 정리 후 작성 직전 HEAD: `431e2f2016189f174da7efbe03a2b8451a423d1e`

### 범위
- RCU ↔ 중앙통제장치(MCE)
- 중앙통제가 다른 내부 장치로 재배포하는 DDS는 본 Binding 범위 밖
- RCUHeartBeatReportType 5 Hz는 중앙통제 HMI 기능으로 노출하지 않음
- SSS 공통 규칙은 중복 이슈로 다시 세지 않음

### 완료
- 중앙통제 Destination: `0x06 / 0x01 / 0x00`
- `System.Target.CentralControl`
- 구형 `MonitorBindingDDS/ControlBindingDDS/Channel` → 공통 Binding 구조
- CommandStatus 처리상태 규칙 통일
- OperationMode `0~7`: 8개 독립 Control + UInt8 FixedField
- AuthorityControl `command 0~4`: 5개 독립 Control + UInt8 FixedField
- command=0 `commandTarget`: 0 고정형 원격통제 / 1 이동형 원격통제 / 2 출입항보조
- 무선 Preset topology/mission: 각 octet Bit7~6 → `PackedField width=8`, `BitMember offset=6 width=2`
- ProcessorIBIT: `cpuTemperature(Float64)`, `cpuLoad(Float64)`
- VHF 원문 Type `VHFchannelCommendType` 철자 보존, 0=중지/1=송신, `rxChannel` long→Int32
- Volume/Squelch selector 1/2 별도 Control + FixedField
- 영상분배: `videoTransmitType=0` → `stopVideoDistribution`, 선택은 1~4 감시#1~#4
- CBIT 표시명: `중앙통제장치 상태`

### 중앙통제 고유 Remaining TBD — 9개
1. `AuthorityControl.destination` 업무 의미
2. VHF `rxChannel` 값 체계/단위/채널표
3. `MCEPBITReportType` 1 Hz 표기의 실제 송신주기
4. PBIT/IBIT Result correlation
5. `MCEIBITReportType.operationalStaus` 원문 철자 vs 실제 IDL
6. `VoiceRxTxControlType.voiceRxControl` 설명 불일치
7. `ControlCommandReportType` 원본 `#REF!`
8. USVHeartBeat 일부 세부 enum raw code 미확정
9. `operationalStateUSV` 2~7 의미 미확정 (0=정상, 1=경고만 확인)

CDM 명칭/계층은 전역 후속 감사 대상.

**주의:** 이 채팅에서 사용자가 `중앙통제장치 CSCI.csv`를 업로드했으나 런타임 `/mnt/data`와 File Search에서 정상 노출되지 않는 도구 문제가 있었다. 새 채팅에서 중앙통제 원문을 다시 세부 검증하려면 CSV를 다시 업로드하거나 File Library에서 직접 확인한다.

---

## 8. Git 상태

작성 직전:
- `main`: `f131851f3f59ad9da986eb2cc2b33f6beed5adc0`
- 작업 브랜치: `feature/usv-semantic-binding-audit`
- 작업 브랜치 HEAD: `431e2f2016189f174da7efbe03a2b8451a423d1e`
- main 대비 27 ahead / 0 behind

작성 직전 main 대비 변경 파일은 6개:
- `ProtocolXml/USV/SideScanSonar/SideScanSonarBinding.xml`
- `ProtocolXml/USV/SideScanSonar/SideScanSonarSemantic.xml`
- `ProtocolXml/USV/SideScanSonar/SideScanSonar_OpenIssues.md`
- `ProtocolXml/USV/CentralControl/CentralControlBinding.xml`
- `ProtocolXml/USV/CentralControl/CentralControlSemantic.xml`
- `ProtocolXml/USV/CentralControl/CentralControl_OpenIssues.md`

이 HANDOFF 문서 추가로 브랜치 HEAD와 ahead 수는 1 증가할 수 있으므로 **새 채팅에서는 반드시 재확인한다.**

USV 브랜치는 아직 main에 병합하지 않았다.

---

## 9. 새 채팅에서 먼저 확인할 파일

```text
ProtocolXml/Docs/UMS_Semantic_Binding_Design_Rules_20260826.md
ProtocolXml/Docs/MCMC2_Semantic_Binding_HANDOFF_20260828.md
ProtocolXml/XSD/CommonSpecSchema.xsd
ProtocolXml/XSD/CommonBindingSchema.xsd
ProtocolXml/XSD/VehicleSpecificationSchema.xsd
ProtocolXml/USV/UsvSpecification.xml
ProtocolXml/USV/SideScanSonar/SideScanSonar_OpenIssues.md
ProtocolXml/USV/CentralControl/CentralControl_OpenIssues.md
```

---

## 10. 다음 장치 작업 방식

- SSS/중앙통제에서 이미 해결한 공통 이슈를 다시 사용자에게 나열하지 않는다.
- 공통 규칙은 자동 적용한다.
- 해당 장치 **고유 이슈만 최종 결과에 보고**한다.
- 애매한 것은 OpenIssues에 남기고 가능한 작업은 진행한다.
- 장치 하나를 가능한 한 한 번에 분석 → 수정 → XSD/구조 정합성 감사까지 끝낸다.

---

## 11. USV 전체 완료 후 전역 최종 감사

1. CDM 이름/계층 일관성
2. dotted-path 구조체 접근 규칙
3. 공용 Destination/USVMessageBase/commandID 규칙
4. commandID 없는 결과 Report correlation
5. 조건부 Parameter 표현 방식
6. 로컬/중복 XSD 제거 여부
7. Specification의 SemanticPath/BindingPath 실제 로딩 규칙
8. USV 전체 converter 잔존
9. 전체 primitive dataType 누락
10. 모든 Control/Reply/Monitor/Product cross-reference

---

## 12. 하지 말아야 할 것

- 원문 없이 enum 의미 생성
- raw code를 Semantic에 남기기
- converter로 packing/scale/자료형 숨기기
- 기존 주석 대량 삭제
- XML 전체 자동 포맷
- CommandStatus ACK를 물리 완료로 해석
- PBIT/IBIT 한 결과 메시지를 내부 필드마다 여러 Reply로 분리
- 모든 IBIT bit를 일괄 PASS/FAIL로 해석
- 상태값이 제어값과 같을 것이라는 이유만으로 raw code 확정
- 최종 CDM 감사 전 새 CDM 무분별 생성
- main에 직접 수정
- OM 실제 구현을 Semantic/Binding XML에 끌어넣기

---

## 13. 새 채팅 시작용 프롬프트

> 이 MD는 `기뢰전 지휘통제 설계`의 최신 Semantic/Binding 작업 HANDOFF야. 파일 전체를 먼저 읽고 현재 Git 상태와 설계 규칙을 이해해. GitHub는 `pepsimanpa/mcmc2`, 현재 작업 브랜치는 `feature/usv-semantic-binding-audit`이야. 수정 전에 반드시 최신 branch HEAD와 `ProtocolXml/Docs/UMS_Semantic_Binding_Design_Rules_20260826.md`, 대상 장치 Semantic/Binding/OpenIssues, 내가 제공하는 원 CSCI/공용 CSV를 실제로 확인해. SSS와 중앙통제에서 이미 해결된 공통 이슈는 다시 설명하지 말고 동일 규칙을 자동 적용해. 기존 주석을 지우지 말고, 애매한 사항은 주석/OpenIssues에 남긴 채 가능한 작업은 진행해. 원문 근거 없이 추정하지 말고 장치 고유 이슈만 최종 결과로 알려줘. 다음 USV 장치를 한 번에 분석→수정→XSD/구조 정합성 감사까지 진행하자.

---

## 현재 상태 한 문장

**AUV는 main 병합 완료. USV는 `feature/usv-semantic-binding-audit`에서 SSS와 CentralControl의 선언형 Semantic/Binding 정리 및 구조 감사까지 완료했으며, 장치 고유 비차단 TBD는 OpenIssues에 보존되어 있다. 다음 USV 장치를 같은 규칙으로 한 번에 처리하면 된다.**


---

## 20. 2026-08-28 USV 최종 통합감사 업데이트

> 이 섹션은 본 HANDOFF 상단/중간의 초기 시각 기준 USV 진행상태를 supersede한다. 실제 작업 시작 전에는 항상 GitHub 최신 HEAD를 다시 확인한다.

- RCU 직접 연동 USV 14개 장치의 Semantic/Binding 설계 및 CSV 재심층 감사 완료.
- 14개 Semantic + 14개 Binding XSD PASS.
- Control/Reply/Monitor/Product cross-reference 오류 0.
- converter 0, source-confirmed primitive `dataType` 미지정 0.
- 최신 PackedField 총수 111.
- 실제 DDS/IDL Boolean direct field 총 11: EO 8 / EOP 1 / NetworkAbstraction 2 / PlatformController 0.
- PBIT/IBIT 실제 Result Reply 61개, commandID 포함 0개 → `USV-COMMON-01`.
- 최종 공통/장치 고유 OpenIssue master: `ProtocolXml/Docs/USV_OpenIssues_Consolidated_20260828.md`.
- 최종 통합 감사: `ProtocolXml/Docs/USV_Semantic_Binding_Integrated_Audit_20260828.md`.
- Active common master issue 10개, resolved common 2개.
- Semantic/Binding 설계 자체는 Review/Merge Ready. Runtime 연동 전 `USV-COMMON-01/03/08/09` 우선 확정 필요.
- SideScanSonar 문서의 과거 `최종 CDM 감사`는 통합감사 완료로 RESOLVED/superseded.
- 현재 남은 장치 고유 TBD는 raw code/unit/bit polarity/운용 규칙/배치값/IDL 확인이 필요한 항목이며 원문 없이 임의로 닫지 않는다.
