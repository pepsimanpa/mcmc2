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

## 3. Remaining TBD

1. **PBIT/IBIT Result correlation**
   - 요청에는 commandID가 있으나 CNEPBIT/CNEIBIT 결과에는 commandID가 없다.
   - CommandStatus ACK 이후 결과와 요청을 연결하는 런타임 규칙은 Adapter 확인이 필요하다.

2. **`integratedNavigationAidedSensor` 조합 여부**
   - 원문 key는 0,1,2,4,8,16으로 정의되어 bit mask처럼 보이지만 조합값 사용 가능 여부가 명시되지 않았다.
   - 현재 Semantic은 원문에 정의된 6개 key만 논리값으로 노출한다.

3. **`ajAsStatus` 정상/동시감지 값**
   - 원문에는 1=GPS 항재밍 감지, 2=항기만 감지만 존재한다.
   - 정상상태 또는 두 상태 동시 발생 표현은 원문에서 확인되지 않는다.

4. **`imuIbit2` reserved 범위 표기**
   - 원문이 `Bit 0?10 : Reserved`로 기재되어 정확한 구간 표기 문자가 훼손되어 있다.
   - 의미가 명확한 Bit 11 이상만 모델링하고 reserved 범위는 임의 확정하지 않는다.

5. **`dvlIbit2` reserved 범위 표기**
   - 원문이 `Bit 20?31 : Reserved`로 기재되어 있다.
   - 의미가 명확한 Bit 0~19만 모델링한다.

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

- 원통 직접 연동 범위의 Semantic/Binding 구조 정리는 완료하였다.
- 남은 8건은 사용자 정책 선택사항이 아니라 추가 CSCI/IDL/Adapter 근거가 필요한 비차단 TBD이다.
