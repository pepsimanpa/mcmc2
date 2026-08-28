# NearContactDetection Semantic/Binding 감사 및 Open Issues

## 1. 범위

- 대상 경계: 원격통제장치(RCU) ↔ 근거리접촉물탐지장치(SCDE).
- 근거리접촉물탐지장치는 RCU와 직접 연동된다. RCU 직접 대상은 서라운드뷰 RTP, SCDE/Lidar CBIT·PBIT·IBIT, CommandStatus ACK 및 공통 Integration/Restart/PBIT/IBIT Control이다.
- ContactReport/WaveReport와 라이다 정보처리 CSC 전용 RTP는 RCU 직접 접점이 아니므로 제외한다.

## 2. 이번 감사에서 확정/반영

- 공용 Control destination은 `System.Target.NearContactDetection`으로 준비하고 물리 식별자는 SurfaceDetection 0x03 / NearContactDetection 0x04 / subEquipment 0x00을 사용한다.
- SCDE/Lidar 상태 0=Normal, 1=Degraded, 2=Unavailable, 3=NoResponse를 Semantic 논리값 + Binding ValueMap으로 분리하였다.
- SCDE IBIT에서 원문에 bit 의미가 직접 명시된 PowerManaging, ECU1~3, T1Converter1~2, EthernetHub, 4D Radar1, RGB Contact1, RGB SVM1만 PackedField로 분해하였다.
- 4D Radar2~9, RGB Contact2~9, RGB SVM2~6는 동일 구조를 추정하지 않고 UInt8 Raw로 유지하였다.
- Lidar IBIT category(UInt16), level(UInt8), sensorAction(UInt8)은 원문 bit 위치를 PackedField로 선언하고 mapping ID는 UInt32로 반영하였다.
- PBIT/IBIT는 CommandStatus 처리 ACK와 SCDE/Lidar 실제 결과 Reply를 분리하였다.
- 기존 header/commandID converter를 제거하고 공통 XSD 경로를 정리하였다.

## 3. Remaining TBD

1. **공통 PBIT/IBIT 요청의 SCDE/Lidar 결과 fan-out 규칙**: 하나의 RCU 요청 후 SCDE 결과와 내부 Lidar 결과가 모두 오는 운용 규칙 및 source routing을 Adapter에서 재확인할 필요가 있다.
2. **PBIT/IBIT Result correlation**: 전용 결과 메시지에 commandID가 없어 처리 ACK 이후 요청과 결과의 런타임 연결 규칙 확인이 필요하다.
3. **반복 장치 상세 bit 의미**: 4D Radar2~9, RGB Contact2~9, RGB SVM2~6는 원문에 개별 bit 설명이 없어 1번 장치 구조를 복제하지 않았다.
4. **Lidar level/action flag 조합 규칙**: bit 위치는 명확하지만 Level 및 SensorAction의 다중 bit 동시 설정 가능 여부/우선순위는 원문에 없다.
5. **서라운드뷰 RTP IP/Port**: 스트림 존재와 RCU 목적지는 확인되나 구체 IP/Port 설정은 별도 ICD/설정 근거가 필요하다.

## 4. 현재 상태

- 원통 직접 연동 범위의 Semantic/Binding 정리는 완료하였다.
- 남은 5건은 사용자 정책 선택사항이 아니라 추가 ICD/Adapter/원문 확인이 필요한 비차단 TBD이다.
