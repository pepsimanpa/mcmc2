# IntegratedNavigation Semantic/Binding 감사 및 Open Issues

## 1. 범위

- 대상 경계: 원격통제장치(RCU) ↔ 복합항법장치(CNE).
- 근거: `복합항법장치 CSCI.csv`, 원격통제장치 CSCI, 공용 구조체/규칙/식별자 규칙.
- RCU 직접 접점인 항법정보, PBIT/CBIT/IBIT, CommandStatus ACK, 공통 제어, RTCM 보정정보 입력을 포함한다.
- `WaterCurrentReportType`과 AIS raw UDP는 RCU 직접 접점이 아니므로 제외한다.

## 2. 이번 감사에서 확정/반영

- 목적지 식별자는 `Navigation=0x01 / NavProcessorCard=0x01 / subEquipment=0x00`이며 공용 Control destination은 `System.Target.IntegratedNavigation`으로 준비한다.
- NavigationData primitive type을 원문대로 반영하였다: sequence UInt8, 위치/속도/자세/IMU Float64, IDL long 계열 Int32.
- 통합항법 상태, 사용 측정치, GNSS 상태, 항재밍·항기만 상태 raw code를 Semantic에서 제거하고 Binding `ValueMap`으로 이동하였다.
- PBIT/CBIT의 각 2-bit 상태를 `PackedField/BitMember`로 분해하여 통신/데이터무결성/BIT 상태를 논리 상태로 노출하였다.
- IBIT `sysIbit`, `imuIbit`, `imuIbit2`, `dvlIbit`, `dvlIbit2`에서 원문에 의미가 명시된 bit만 선언하였다.
- `imuIbit2`의 11~15,20~23 bit는 OK/Error, 16~19는 counter, 24~31은 가속도계 온도로 분리하였다.
- `dvlIbit2`의 0~19 bit는 각 Beam/축의 Invalid/Valid 상태로 분리하였다.
- PBIT/IBIT는 처리 ACK와 전용 결과 Reply를 분리하고 각 결과 메시지는 하나의 결과 그룹으로 유지하였다.
- RTCM `vaildRtcmDataSize`는 기존 합의대로 Semantic 0~256 byte를 유지하고 Binding의 물리 오탈자는 보존하였다. wire type은 Int32, `rtcmDataBuf`는 256-byte 배열로 선언하였다.
- 기존 `BuildUSVMessageBase`, `UInt16` converter를 제거하였다.
- 복합항법 CSCI 재심층 감사에서 `imuIbit2`의 `Bit 0?10 : Reserved`는 동일 UInt32 내 Bit11~31이 연속 정의되어 있어 `Bit 0~10 : Reserved`로, `dvlIbit2`의 `Bit 20?31 : Reserved`는 Bit0~19가 연속 정의되어 있어 `Bit 20~31 : Reserved`로 확정하였다. 두 `?`는 구간기호가 훼손된 원문/CSV 표기 결함으로 정리한다.

## 3. Remaining TBD / Resolved — 미해결 6개

1. **PBIT/IBIT Result correlation**
   - 요청에는 commandID가 있으나 CNEPBIT/CNEIBIT 결과에는 commandID가 없다.
   - CommandStatus ACK 이후 결과와 요청을 연결하는 런타임 규칙은 Adapter 확인이 필요하다.

2. **`integratedNavigationAidedSensor` 조합 여부**
   - 원문 key는 0,1,2,4,8,16으로 정의되어 bit mask처럼 보이지만 조합값 사용 가능 여부가 명시되지 않았다.
   - 현재 Semantic은 원문에 정의된 6개 key만 논리값으로 노출한다.

3. **`ajAsStatus` 정상/동시감지 값**
   - 원문에는 1=GPS 항재밍 감지, 2=항기만 감지만 존재한다.
   - 정상상태 또는 두 상태 동시 발생 표현은 원문에서 확인되지 않는다.

4. **[RESOLVED] `imuIbit2` reserved 범위 표기**
   - 원문은 `Bit 0?10 : Reserved`로 기재되어 있으나 같은 UInt32에서 Bit11~15, Bit16~19, Bit20~23, Bit24~31이 연속적으로 모두 정의되어 있다.
   - 따라서 남는 하위 11bit는 Bit0~10뿐이며 `?`는 구간기호 훼손으로 판단하여 Reserved 범위를 Bit0~10으로 확정한다. Reserved bit는 Semantic에 노출하지 않는다.

5. **[RESOLVED] `dvlIbit2` reserved 범위 표기**
   - 원문은 `Bit 20?31 : Reserved`로 기재되어 있으나 같은 UInt32에서 Bit0~19가 개별 의미로 연속 정의되어 있다.
   - 따라서 나머지 상위 12bit는 Bit20~31이며 `?`는 구간기호 훼손으로 판단한다. Reserved bit는 Semantic에 노출하지 않는다.

6. **IMU 가속도계 온도 signedness**
   - `imuIbit2` Bit24~31이 Accelerometer Temperature, LSB=1°C로 정의되지만 signed/unsigned 및 유효 범위가 없다.
   - Semantic은 온도 단위만 유지하고 Range는 지정하지 않는다.

7. **IDC 온도/CPU Load 유효 범위**
   - `idcTemperature`와 `idcCpuLoad`의 Range 열이 각각 `0`으로만 기재되어 실제 유효 범위인지 미기재 표기인지 불명확하다.
   - wire Float32 및 Unit만 반영하고 Semantic Result Range는 지정하지 않는다.

8. **RTCM valid size 원문 Range 불일치**
   - `vaildRtcmDataSize` 원문 범위 0~1,000,000,000은 `rtcmDataBuf[256]`과 충돌한다.
   - 기존 설계 결정대로 실제 버퍼 내 유효 byte 수 의미에 맞춰 Semantic 0~256을 유지하며, 추가 IDL/Adapter 확인 시 재검토한다.

## 4. 현재 상태

- 원통 직접 연동 범위의 Semantic/Binding 구조 정리와 복합항법 CSCI 재심층 감사까지 완료하였다.
- reserved 구간 표기 2건은 32-bit 전체 bit 배치와 연속 정의를 근거로 해소하였다.
- 미해결 6건은 사용자 정책 선택사항이 아니라 추가 CSCI/IDL/Adapter 근거가 필요한 비차단 TBD이다.
