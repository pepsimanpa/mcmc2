# NetworkAbstraction Semantic/Binding 감사 및 Open Issues

## 1. 범위

- 대상 경계: 원격통제장치(RCU) ↔ 네트워크추상화장치 CSCI.
- 근거: `네트워크추상화장치 CSCI.csv`, `원격통제장치 CSCI.csv`, `공용 구조체.csv`, `공용 규칙.csv`, `공용 식별자 규칙.csv`.
- 중앙통제장치 등 다른 내부 장치 전용 연동은 원통 직접 접점 범위에서 제외한다.
- 동일 장비군이라도 통제소/USV/L-Band/GeoSat/Aux 대상이 물리 `subEquipmentID`로 구분되는 경우 대상별 Control을 유지한다.

## 2. 이번 감사에서 확정/반영

- 통신장비 계열 `equipmentType=0x08`, 네트워크추상화장치 `equipmentID=0x01`을 기준으로 대상별 prepared Destination을 분리하였다.
  - NAE 통제소 `0x01`
  - NAE USV `0x02`
  - L-Band 통제소/중계소 그룹 `0x11`
  - L-Band USV 단말 `0x12`
  - GeoSat 통제소 `0x21`
  - GeoSat USV `0x22`
  - 출입항보조 송수신기 USV `0x31`
- 원통 직접 Control 19개, Reply 32개, Monitor 9개 구조를 원문과 대조하였다.
- 통신 링크 추가/삭제의 `commsChannelID`는 원문에서 정의된 `0=무선`, `1=위성`만 Semantic 논리값으로 노출하고 Binding `ValueMap`으로 이동하였다. Range에만 존재하고 의미가 없는 값 2는 포함하지 않는다.
- `NetworkIntegrationDevice*BIT`와 L-Band/Aux/GeoSat 상태 enum raw code를 Semantic에서 제거하고 Binding `ValueMap`으로 이동하였다.
- L-Band `operation` 상위 2비트 추출은 converter 대신 `PackedField width=8 / BitMember offset=6 width=2`로 선언하였다.
- CyberThreat의 위협 종류 하위 2비트 추출도 converter 대신 `PackedField`로 선언하였다.
- L-Band throughput 0.1 단위와 GeoSat k/M 단위 변환을 declarative `scale`로 변경하였다.
- 기존 `BuildUSVMessageBase`, `UInt16`, `Mask0x03`, `ShiftRight6Mask0x03`, `MultiplyBy0.1`, `MultiplyBy1000`, `MultiplyBy1000000` converter를 제거하였다.
- 공용 PBIT/IBIT 요청 destination은 대상별 `System.Target.NetworkAbstraction.*` prepared composite로 변경하였다.

## 3. Remaining TBD

1. **GeoSat 통신상태 8개 필드의 실제 DDS primitive type**
   - `modulatorStatus.valid`와 `demodulatorStatus.valid`는 원 DDS/IDL boolean으로 확인되어 `dataType="Boolean"`으로 확정하였다.
   - 남은 `outputLevel`, `dataBW`, `dataRate`, `signalPower`, `cn`, `ber`, `bitRate`, `trafficRate` 8개 수치 필드의 실제 IDL primitive type은 현재 근거만으로 안전하게 재확정하지 못했다.
   - 단위/scale은 원문에 따라 유지하되 남은 8개 `dataType`은 임의 지정하지 않는다.

2. **`commsChannelID=2` 의미**
   - Add/Delete 메시지의 Range는 0~2이나 직접 정의된 값은 0=무선, 1=위성뿐이다.
   - 값 2의 의미가 확인되기 전까지 Semantic/ValueMap에 포함하지 않는다.

3. **PBIT/IBIT Result correlation 및 대상 식별**
   - 여러 점검 요청은 `DestinationType.subEquipmentID`로 통제소/USV/장비군을 구분하지만 전용 Result Report에는 commandID 또는 동일한 대상 식별자가 없는 경우가 있다.
   - 요청-결과 correlation 및 통제소/USV 결과 routing 규칙을 Adapter/IDL에서 확인해야 한다.

4. **L-Band 통제소/중계소 그룹 결과 식별**
   - 통제소 1대와 중계소 5대 상태가 하나의 그룹 Report에 포함된다.
   - 개별 중계기 장애/무응답과 전체 Report 미수신을 런타임에서 어떻게 구분하는지 확인이 필요하다.

5. **`IntraNetworkStatusType.commandID`의 생산/업무 의미**
   - 상태 Report에 commandID가 존재하나 일반 명령 ACK correlation과 동일한 의미인지 원문에서 충분히 명확하지 않다.
   - 모니터 상태의 업무값으로 확대 해석하지 않는다.

6. **`configureLBandComms`의 packed control octet 논리화 범위**
   - `BaseStationCommsCommonCtrl` / `ModemCommsCtrl`의 `netSet`, `handoverMode`, `output`, `comFA`, `topology`, `resource`, `debug`, `fa`, `topology`, `amc`는 bit-packed octet으로 원문 bit 위치가 존재한다.
   - 현재 확보된 근거에서는 일부 1-bit 필드의 논리 polarity/표시명까지 완전하게 재확정되지 않아 Semantic의 해당 제어값은 `*.Raw` Profile로 유지하였다.
   - raw bit 위치는 Binding/주석에 보존하며, 원 CSCI/IDL의 정확한 라벨·polarity가 다시 확인되면 `ValueSetProfile + PackedField/BitMember`로 최종 논리화한다.

7. **GeoSat `valid=false` 하위값 처리**
   - 원문상 Modulator/Demodulator `valid=false`이면 해당 하위 데이터는 신뢰하지 않는다.
   - UI/OM이 invalid 상태에서 하위 수치를 숨길지, 마지막 값을 표시할지 등의 표현 정책은 Semantic/Binding 밖의 런타임 규칙으로 확인이 필요하다.

8. **공용 Network/L-Band/GeoSat/Aux Result source routing**
   - 여러 subEquipment가 동일/유사 Report Type을 공유하는 구간이 있다.
   - `USVMessageBase`와 Topic 분리만으로 충분한지, Adapter가 추가 source context를 사용해야 하는지 확인이 필요하다.

## 4. 현재 상태

- 원통 직접 연동 구조, raw enum 분리, converter 제거, 확정 가능한 primitive type 보강은 완료하였다.
- `configureLBandComms`의 일부 `*.Raw` Semantic Profile과 GeoSat 수치 8개 `dataType`은 근거 부족 때문에 의도적으로 미확정 상태로 보존한다.
- 남은 8건은 사용자가 임의로 결정할 사항이 아니라 추가 CSCI/IDL/Adapter 근거가 필요한 비차단 TBD이다.
