# NearContactDetection Semantic/Binding 감사 및 Open Issues

## 1. 범위

- 대상 경계: 원격통제장치(RCU) ↔ 근거리접촉물탐지장치(SCDE).
- 근거리접촉물탐지장치는 RCU와 직접 연동된다. RCU 직접 대상은 서라운드뷰 RTP, SCDE/Lidar CBIT·PBIT·IBIT, CommandStatus ACK 및 공통 Integration/Restart/PBIT/IBIT Control이다.
- ContactReport/WaveReport와 라이다 정보처리 CSC 전용 RTP는 RCU 직접 접점이 아니므로 제외한다.

## 2. 이번 감사에서 확정/반영

- 공용 Control destination은 `System.Target.NearContactDetection`으로 준비하고 물리 식별자는 SurfaceDetection 0x03 / NearContactDetection 0x04 / subEquipment 0x00을 사용한다.
- SCDE/Lidar 상태 0=Normal, 1=Degraded, 2=Unavailable, 3=NoResponse를 Semantic 논리값 + Binding ValueMap으로 분리하였다.
- SCDE IBIT의 PowerManaging, ECU1~3, T1Converter1~2, EthernetHub를 원문 bit 정의대로 PackedField로 분해하였다.
- 반복 센서군은 CSV 첫 행 비고가 장비번호 `1`이 아니라 `#` placeholder(`4D 이미지 레이더#`, `RGB 카메라(접촉물)#`, `RGB 카메라(서라운드뷰)#`)로 전원/동작 상태 bit를 정의하고 이후 동일 센서 행의 비고가 비어 있는 병합셀 export 패턴임을 재확인하였다. 이에 Radar1~9, RGB Contact1~9, RGB SVM1~6 전체에 bit0=전원, bit1=동작, 0=비정상/1=정상 규칙을 동일하게 반영하였다.
- Lidar IBIT category(UInt16), level(UInt8), sensorAction(UInt8)은 원문 bit 위치를 PackedField로 선언하고 mapping ID는 UInt32로 반영하였다.
- 2026-08-28 재업로드된 `근거리접촉물탐지장치 CSCI.csv`를 전체 재검증하였다. SCDE 반복 센서의 `#` placeholder와 0=비정상/1=정상 규칙, Lidar1/2 Category·Level·SensorAction bit 위치, SCDE/Lidar CBIT·PBIT·IBIT 상태코드 0~3을 재확인하였다.
- 같은 업로드본에서 PBIT/IBIT 요청 1회에 SCDE/Lidar 결과가 모두 발생한다는 fan-out 규칙, 결과 commandID correlation, Lidar level/action 다중 bit 우선순위, 서라운드뷰 RTP 실제 IP/Port는 추가 정의를 찾지 못했다. 따라서 미해결 4건은 그대로 유지한다.
- Binding에 남아 있던 "반복 센서 2~N은 미확정" 및 "Lidar bit-packed 값을 raw 유지"라는 과거 주석은 실제 PackedField 구현/해결 상태와 충돌하여 현재 근거에 맞게 동기화하였다.
- PBIT/IBIT는 CommandStatus 처리 ACK와 SCDE/Lidar 실제 결과 Reply를 분리하였다.
- 기존 header/commandID converter를 제거하고 공통 XSD 경로를 정리하였다.

## 3. Remaining TBD / Resolved — 미해결 4개

1. **공통 PBIT/IBIT 요청의 SCDE/Lidar 결과 fan-out 규칙**: 하나의 RCU 요청 후 SCDE 결과와 내부 Lidar 결과가 모두 오는 운용 규칙 및 source routing을 Adapter에서 재확인할 필요가 있다.
2. **PBIT/IBIT Result correlation**: 전용 결과 메시지에 commandID가 없어 처리 ACK 이후 요청과 결과의 런타임 연결 규칙 확인이 필요하다.
3. **[RESOLVED] 반복 장치 상세 bit 의미**: 각 센서군 첫 행의 비고가 `4D 이미지 레이더#`, `RGB 카메라(접촉물)#`, `RGB 카메라(서라운드뷰)#`처럼 `#` placeholder로 작성되어 특정 1번 센서 전용이 아닌 반복 센서 공통 bit 계약임을 확인하였다. 이후 2~N 행의 비고 공란은 병합셀 CSV export 패턴으로 판단하며, Radar1~9 / Contact1~9 / SVM1~6 전체에 bit0=전원상태, bit1=동작상태, 0=비정상/1=정상 규칙을 적용하였다.
4. **Lidar level/action flag 조합 규칙**: bit 위치는 명확하지만 Level 및 SensorAction의 다중 bit 동시 설정 가능 여부/우선순위는 원문에 없다.
5. **서라운드뷰 RTP IP/Port**: 스트림 존재와 RCU 목적지는 확인되나 구체 IP/Port 설정은 별도 ICD/설정 근거가 필요하다.

## 4. 현재 상태

- 원통 직접 연동 범위의 Semantic/Binding 정리와 재업로드된 근거리접촉물탐지장치 CSCI 전체 재검증까지 완료하였다.
- 반복 센서 상세 bit 의미는 CSV 공통 placeholder/병합셀 구조 근거로 해소되어 있으며, Binding의 과거 주석도 현 구현과 동기화하였다.
- 재업로드본에서도 추가로 닫을 수 있는 항목은 확인되지 않았다. 미해결 4건은 추가 ICD/Adapter/운용 설정 근거가 필요한 비차단 TBD이다.
