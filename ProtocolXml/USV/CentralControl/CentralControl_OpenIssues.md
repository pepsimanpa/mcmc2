# CentralControl Semantic/Binding 최종 감사

## 1. 범위

- 대상 경계: 원격통제장치(RCU) ↔ 중앙통제장치(MCE).
- 중앙통제가 자율운항/선체제어/네트워크/영상장치로 재배포하는 내부 DDS는 본 Binding 범위에서 제외한다.
- 원통→중통 `RCUHeartBeatReportType` 5 Hz는 원격통제장치 생존정보이므로 중앙통제 HMI 기능으로 노출하지 않는다.
- SSS에서 확정한 공통 규칙(생성 header/commandID, Derived destination, dotted-path struct 접근, CommandStatus 처리상태, converter 금지, PBIT/IBIT 요청-결과 분리)은 중앙통제에도 동일 적용하며 별도 Open Issue로 중복 관리하지 않는다.

## 2. 이번 패스에서 확정/정리

- 공용 식별자 규칙에 따라 중앙통제 일반 대상은 `dstEquipmentType=0x06(CoreProcessing)`, `dstEquipmentID=0x01(CentralManagementControlCard)`, `dstSubEquipmentID=0x00`으로 확정하고 `System.Target.CentralControl` Derived destination을 사용한다.
- 기존 `MonitorBindingDDS/ControlBindingDDS/Channel`을 공통 `MonitorBinding/ControlBinding/DDSChannel` 형식으로 통일한다.
- `CommandStatusReportType`은 SSS와 동일하게 `dstEquipmentType/dstEquipmentID/status/commandID`를 Binding에서 보존하고 Semantic에는 처리상태 `status`만 노출한다. 완료/성공 응답으로 해석하지 않는다.
- `OperationModeControl.operationMode` 0~7은 기존 8개 독립 Control + UInt8 FixedField 구조를 유지한다.
- `AuthorityControl.command` 0~4는 기존 5개 독립 Control + UInt8 FixedField 구조를 유지한다. command=0의 `commandTarget`은 `0=고정형 원격통제`, `1=이동형 원격통제`, `2=출입항보조` ValueSet으로 표현한다.
- 무선 Preset은 topology와 mission이 각각 독립 octet이고 각 octet의 Bit7~6을 사용하므로 `PackedField(width=8) / BitMember(offset=6,width=2)`로 선언한다. converter는 사용하지 않는다.
- `ProcessorIBIT` 공용 16-byte 구조를 확인하여 `cpuTemperature(Float64)`와 `cpuLoad(Float64)`를 IBIT 결과에 노출한다.
- VHF `VHFchannelCommendType`은 원문 철자를 보존하고 `0=중지`, `1=송신` ValueSet으로 표현한다. `rxChannel`의 물리 자료형은 4-byte long → `Int32`로 반영한다.
- Volume/Squelch selector 1/2는 기존처럼 별도 Control + FixedField로 유지한다.
- 영상분배는 확정된 프로젝트 결정대로 `videoTransmitType=0`을 `stopVideoDistribution`으로 분리하고, `selectVideoDistribution`은 `1~4=감시#1~#4` 선택만 노출한다.
- 중앙통제 CBIT Monitor 표시명은 확정된 운용자 관점 명칭인 `중앙통제장치 상태`로 정리한다.

## 3. 중앙통제 고유 Remaining TBD

1. **AuthorityControl.destination 업무 의미**: 3-byte 물리 구조는 확정되어 있으나 통제권 업무에서 해당 destination이 누구/무엇을 지칭하는지 원문 설명이 부족하다. Semantic 입력으로 올리지 않고 BindingOnly로 유지한다.
2. **VHF rxChannel 값 체계**: `long` 4-byte 및 '채널/채널 및 주파수' 설명만 확인된다. 허용 범위, 주파수 단위, 채널표는 원문만으로 확정할 수 없다.
3. **MCEPBITReportType 1 Hz 표기**: 이름/원통 요청 구조는 PBIT 결과 응답 형태이나 중앙통제 자료에는 1 Hz 표기가 존재한다. 현재는 요청의 전용 Result Reply로 유지하고 실제 송신주기 확인이 필요하다.
4. **PBIT/IBIT Result correlation**: 요청에는 commandID가 있으나 MCEPBIT/MCEIBIT 결과에는 commandID가 없다. CommandStatus ACK는 commandID로 연결되지만 전용 결과의 런타임 매칭 규칙은 확인이 필요하다.
5. **MCEIBITReportType.operationalStaus 철자**: 중앙통제 자료의 `operationalStaus`를 그대로 보존한다. 실제 DDS IDL이 `operationalStatus`인지 확인 필요하다.
6. **VoiceRxTxControlType.voiceRxControl 설명 불일치**: 필드명/설명은 수신인데 비고의 raw 1 설명은 '송신'으로 적혀 있다. 0/1 물리값은 유지하되 raw 1의 논리명은 IDL/구현 확인 전 확정하지 않는다.
7. **ControlCommandReportType 원본 #REF!**: controlCommand 필드와 값 의미는 식별 가능하지만 주변 원본 셀의 `#REF!`는 source defect로 남는다.
8. **USVHeartBeat 세부 enum**: `operationalModeUSV` 0~7과 `operationalStateUSV`의 0/1은 확정했다. missionModeAC / operationalModeEOIR / operationalModeTSA / operationalModeRCWS의 raw code는 이번 근거에서 독립적으로 재확인되지 않아 기존 논리 목록만 유지하고 Binding ValueMap은 임의 생성하지 않는다.
9. **operationalStateUSV 2~7**: 원문 범위는 0~7이나 확인된 의미는 0=정상, 1=경고뿐이다. 2~7 의미를 추정하지 않는다.

## 4. 전역 후속 항목

- CDM 명칭/계층은 다른 USV 장치 완료 후 최종 USV CDM 감사에서 일괄 재검토한다. 이는 중앙통제 고유 Open Issue로 세지 않는다.
