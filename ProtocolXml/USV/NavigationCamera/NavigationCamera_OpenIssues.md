# NavigationCamera Semantic/Binding 감사 및 Open Issues

## 1. 범위

- 대상 경계: 원격통제장치(RCU) ↔ 운항용카메라장치.
- 근거: `운항용카메라 CSCI.csv`, 원격통제장치 CSCI, 공용 구조체/규칙/식별자 규칙.
- 중앙통제 전용 운항기록 EO/IR RTP는 원통 직접 접점이 아니므로 제외한다.
- 원격통제소 전시 RTP, 객체탐지 결과, CBIT/PBIT/IBIT, CommandStatus ACK, 공통 제어 및 `CCDDisplayModeControlType`을 포함한다.

## 2. 이번 감사에서 확정/반영

- 목적지 식별자는 `SurfaceDetection=0x03 / NavCameraDevice=0x07 / subEquipment=0x00`이며 공용 Control destination은 `System.Target.NavigationCamera`로 준비한다.
- `panoViewType=0x00`과 객체분류 0~7 raw code를 Semantic에서 제거하고 Binding `ValueMap`으로 이동하였다.
- 영상분석 primitive wire type을 원문/공용 구조체 기준으로 반영하였다: image size UInt16, count/FPS/class/confidence UInt8, bbox UInt16, relativeAngle Float32.
- 상태 code는 `Normal=0 / Unavailable=2 / NoResponse=3`으로 Binding에 선언하고, 원문에서 미사용으로 명시한 `Degraded=1`은 Semantic 유효값에서 제외하였다.
- PBIT/IBIT는 처리 ACK와 전용 결과 Reply를 분리하고, 한 결과 메시지 안의 필드를 하나의 결과 그룹으로 유지하였다.
- `displayMode`는 Bit0 EO, Bit1 IR, Bit2 화질을 Semantic 논리 선택값으로 분리하고 Binding `PackedField`로 선언하였다.
- 기존 `BuildUSVMessageBase`, `UInt16`, `PackNavigationCameraDisplayMode` converter를 제거하였다.
- 원문 근거가 있는 primitive 필드에 wire `dataType`을 반영하였다.

## 3. Remaining TBD

1. **원격 전시 RTP IP/Port**
   - CSCI에 송수신 IP/Port가 `todo`로 남아 있다.
   - 현재 `RTPChannel`은 스트림 존재만 표현한다.

2. **객체탐지 sequence 최대 길이**
   - `imageObjectDetectionCnt`는 0~255이지만 `sequence<ImageObjectDetection2DType>` 자체의 명시적 최대 길이는 원문에 없다.
   - 현재 Collection 최대 255는 count 필드 범위에 맞춘 정합성 가정이다.

3. **PBIT/CBIT 하위 상태 code 적용 근거**
   - `statusCCD`에는 0/2/3 의미가 명시되어 있으나 나머지 7개 상태 필드는 Range 0~3만 기재되어 있다.
   - 동일 상태 구조와 IBIT 명세를 근거로 0=정상, 2=비가용, 3=미응답을 적용했으며 별도 IDL enum 정의가 있으면 재확인한다.

4. **IBIT 상세 octet polarity**
   - 상세 필드는 비트 위치와 점검 대상은 정의되어 있으나 각 bit의 0/1 의미가 명시되지 않았다.
   - 따라서 임의 Boolean 의미를 부여하지 않고 UInt8 Raw로 유지한다.

5. **카메라 2~8 상세 비트 설명**
   - EO1/IR1 상세만 각 bit 의미가 설명되어 있고 EO2~8/IR2~8은 범위만 존재한다.
   - 동일 구조라고 단정하지 않고 Raw로 유지한다.

6. **PBIT/IBIT Result correlation**
   - 요청에는 commandID가 있으나 전용 CCDPBIT/CCDIBIT 결과에는 commandID가 없다.
   - CommandStatus ACK 이후 전용 결과와 요청을 연결하는 런타임 규칙은 Adapter/IDL 확인이 필요하다.

7. **relativeAngle의 -PI~+PI 표기**
   - 공용 구조체의 정확한 범위가 기호 `-PI~+PI`로 정의되어 있으나 Semantic XSD Range는 decimal만 허용한다.
   - 근사 소수값을 계약값으로 만들지 않기 위해 Unit만 유지하고 Range는 주석으로 보존한다.

## 4. 현재 상태

- 원통 직접 연동 범위의 Semantic/Binding 정리는 완료하였다.
- 남은 7건은 사용자 정책 선택사항이 아니라 추가 IDL/Adapter/원문 확인이 필요한 비차단 TBD이다.
