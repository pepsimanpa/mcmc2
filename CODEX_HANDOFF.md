# Codex 작업 인계 — Semantic 식별자, Reply 계약, HMI 문서

- 작성일: 2026-09-22 (KST)
- 저장소: https://github.com/pepsimanpa/mcmc2.git
- 작업 브랜치: feature/control-profile-identity
- 작성 직전 HEAD: 90884d4b8134336c43e8b58795b20f4b67ed91f0
- 원격 브랜치: origin/feature/control-profile-identity
- 목적: 다른 PC의 Codex에서 이번 작업을 같은 설계 의도로 바로 이어가기 위한 인계 문서

이 문서를 먼저 읽고 실제 파일과 Git 상태를 다시 확인한다. ProtocolXml/Docs/MCMC2_Semantic_Binding_HANDOFF_20260828.md는 이전 작업의 역사적 문서이며, 이번 브랜치의 최신 결정은 이 문서를 우선한다.

## 1. 집에서 작업 재개

저장소가 없다면:

    git clone --branch feature/control-profile-identity --single-branch https://github.com/pepsimanpa/mcmc2.git
    cd mcmc2
    git status -sb

이미 저장소가 있다면:

    git fetch origin
    git switch feature/control-profile-identity
    git pull --ff-only
    git status -sb

새 Codex 세션에는 다음과 같이 지시하면 된다.

    먼저 CODEX_HANDOFF.md를 읽고 현재 브랜치와 실제 파일 상태를 검증한 다음,
    남은 작업 목록을 기준으로 이어서 작업해줘.

## 2. 최종 확정한 계층 경계

### Semantic

HMI가 사용하는 공개 계약이다.

- Control/Monitor/ReplySpec의 논리적 기능
- Profile, Result 등의 id, name, cdm
- 단위, 범위, 해상도, 허용 가능한 Value 목록
- HMI가 전송하거나 수신하는 의미값

### Binding

장비 프로토콜 표현을 담당한다.

- Channel, Topic, Type, Field
- wire dataType, 길이, scale, format
- 실제 숫자 enum과 ValueMap
- 장비 메시지의 Reply 구조

### OperationFramework

Binding의 wire 값과 Semantic 공개값 사이를 변환한다.

- HMI는 Binding의 0, 1, 2 같은 값을 알지 않는다.
- Semantic에 wire 숫자나 UInt8 같은 전송 타입을 넣지 않는다.
- Binding의 표시명이나 숫자값을 HMI 계약에 직접 노출하지 않는다.

## 3. HMI Key/Value 최종 계약

이번 대화에서 Value 항목에 별도 id를 추가하는 방안도 검토했으나, 최종 결정은 기존 Control 입력 규칙을 유지하여 Value.name을 공개값으로 사용하는 것이다.

### Control 입력

    Key   = Profile.id
    Value = Value.name 또는 숫자/문자열 실제 입력값

예:

    OperationFramework.Execute("control -a sonarType,TowedSideScanSonar");

여기서 sonarType은 Profile id, TowedSideScanSonar는 Semantic Value의 name이다.

### Monitor 출력

    value.Key   = Semantic 출력 항목 id
    value.Value = ValueSet이면 Value.name, 그 외에는 실제 논리값
    value.Unit  = Semantic 단위

### Reply 출력

    update.ReplyId = ReplySpec.id
    value.Key      = Result.id
                     중첩 GroupResult는 group.child 경로
    value.Value    = ValueSetResult이면 Value.name, 그 외에는 실제 논리값
    value.Unit     = Semantic 단위

예를 들어 EMDW 장비가 result=0을 반환하면:

    Binding wire 0
    → ValueMap CDM Command.Result.Success
    → Semantic Value.name "성공"
    → ReplyUpdate value.Value "성공"

따라서 현재 Value에는 별도 id를 추가하지 않는다. 대신 name을 공개 API 값으로 취급한다.

- 같은 ValueSet 안에서 name은 고유해야 한다.
- name 변경은 공개 API 호환성 변경이다.
- 한글 name이면 callback의 value.Value도 한글이다.
- 다국어 표시가 필요해질 때에만 id/표시명 분리를 다시 검토한다.

## 4. 완료된 XML/XSD 변경

### Control Profile 식별자

- MDV, EMDW, AUV, USV Semantic의 Control 입력 Profile에 id와 name을 정리했다.
- 공통 XSD에서 관련 Profile 식별자를 필수로 강화했다.
- HMI Control 입력 Key는 Binding Field 이름이 아니라 Semantic Profile id다.

### ReplySpec / ReplyBinding 정리

- Semantic의 Reply를 ReplySpec으로 통일했다.
- Binding의 Reply를 ReplyBinding으로 통일했다.
- Semantic ReplySpec과 Result 계층에 필요한 id/name을 추가했다.
- ReplyBinding은 semantic_id로 ReplySpec id와 대응한다.
- bindRef는 XML, XSD, 웹 파서와 문서에서 제거했다.
- 처리상태 Reply와 실제 결과 Reply를 replyId로 구분할 수 있게 했다.

주요 규모:

- Semantic 18개 파일 XSD 검증 완료
- Binding 19개 파일 XSD 검증 완료
- ReplyBinding 303개가 ReplySpec ID와 대응하는지 검증 완료

### XSD 위치

Canonical:

    ProtocolXml/XSD/CommonSpecSchema.xsd
    ProtocolXml/XSD/CommonBindingSchema.xsd

SSS Semantic/Binding은 위 canonical XSD를 직접 참조한다. 미참조 상태였던 SSS 폴더의 로컬 XSD 복사본 두 개는 삭제했다.

Sample 호환 복사본은 별도로 유지한다.

    Sample/OperationManagement/OperationManagement/spec/CommonSpecSchema.xsd

스키마를 추가 변경할 때 canonical과 필요한 Sample 복사본을 함께 확인한다.

## 5. Reply IDL 및 callback 계약

현재 개념 계약은 web/OperationManagement.idl에 반영되어 있다.

ControlExecutionRequest:

- requestId
- targetId
- controlSpec

ControlExecutionReply:

- requestId
- targetId
- controlId
- replyId
- replyCdm
- executionReport
- results: sequence of ControlResult

ControlResult는 fieldName, cdm, dataType, fieldValue, unit을 가지며 OperationFramework가 공개 callback의 Key, Value, Unit으로 변환한다.

예시:

    var replySubscription = OperationFramework.SubscribeReply(
        "sideScanSonar.control.configureOperatingParameters",
        update =>
        {
            // RequestId, ReplyId, State로 응답을 식별한다.
            foreach (var value in update.Values)
            {
                // Key=Semantic Result id, Value=Semantic 값, Unit=Semantic 단위
            }
        });

콜백 등록 1회는 구독을 한 번 등록한다는 뜻이다. callback은 Processing, Finished, Failed 또는 여러 ReplySpec 때문에 여러 번 호출될 수 있다.

스레드 규칙:

- HMI가 Task.Run으로 SubscribeReply를 감싸지 않는다.
- DDS 수신 루프는 OperationFramework 내부에서 관리하는 구조가 적절하다.
- callback이 worker thread에서 호출될 수 있으므로 WPF/WinForms 등은 UI 변경만 Dispatcher/BeginInvoke로 넘긴다.
- 화면 종료 시 subscription을 Dispose한다.

## 6. 웹 HTML/HMI 문서 완료 내용

주요 파일:

    web/app.js
    web/styles.css
    web/OperationManagement.idl

### Semantic 중심 HMI 계약

- Control 입력 ID, 표시명, CDM, 형식, 허용값, 명령 예제는 Semantic에서 만든다.
- Reply 결과 형식과 값 목록도 Semantic ReplySpec에서 만든다.
- Monitor 전시 항목도 Semantic에서 만든다.
- 명령 예제는 Binding wire 값보다 Semantic Value.name을 사용한다.
- 입력 검증은 허용값을 찾은 뒤 최종 전송값을 Value.name으로 정규화한다.

Binding이 직접 표시되는 Binding 상세 화면은 의도적으로 유지한다.

- Binding variant 수
- Binding 파일명
- 전송 방식, Channel, Field
- ReplyBinding 상세

### 화면 순서와 명칭

Control:

1. 01 · HMI 입력 항목
2. 02 · HMI 제어 실행 방법
3. 03 · ReplySpec 출력

Monitor:

1. 01 · HMI 전시 항목
2. 02 · HMI 모니터 구독 방법

Control 입력이 없을 때는 HMI 입력 항목 없음으로 표시한다.

### UI 정리

- 명령 인터페이스와 HMI SW 호출 코드는 가능한 한 한 줄로 표시한다.
- 표 안의 명령 복사, 코드 복사 버튼을 각 코드 블록의 우측 상단에 배치해 행 높이를 줄였다.
- 공개 API 용어는 Reply를 유지하고 XML 모델 용어는 ReplySpec/ReplyBinding으로 구분했다.

## 7. 주요 커밋

이번 브랜치의 커밋은 다음 순서다.

    089b7d8 feat: MDV 제어 파라미터 식별자 추가
    4c07ba6 feat: EMDW 제어 파라미터 식별자 추가
    3ac71d7 feat: AUV 제어 파라미터 식별자 추가
    cbf4df0 feat: USV 제어 파라미터 식별자 추가
    a70bd17 fix: 프로파일 식별자 검증 규칙 강화
    9b22f33 feat: HMI 입력을 Semantic 프로파일 ID로 표시
    a8a98b5 fix: HMI 계약을 Semantic 정의로 분리
    40fe62d style: HMI 명령을 한 줄로 표시
    eab707b feat: Reply Semantic 식별자 및 콜백 계약 추가
    11a0ce9 refactor: ReplySpec과 ReplyBinding 명칭 통일
    7183bd6 refactor: HTML ReplySpec 용어 정렬
    7e69fff refactor: HMI 안내 순서와 명칭 정리
    08495fd refactor: HMI 항목 명칭 명확화
    945a887 refactor: Monitor 전시 항목 명칭 적용
    548aa10 refactor: HMI 복사 버튼을 코드 헤더에 배치
    90884d4 refactor: HMI 입력 빈 상태 문구 통일

## 8. 검증 완료 항목

- Semantic XML: 공통 XSD 기준 18/18 통과
- Binding XML: 공통 XSD 기준 19/19 통과
- ReplyBinding 303개와 ReplySpec ID 대응 확인
- 구형 Reply와 bindRef가 작업 대상 소스에 남지 않았는지 확인
- node --check web/app.js 통과
- git diff --check 통과
- 작성 직전 로컬 HEAD와 원격 브랜치 HEAD 일치 확인

브라우저 자동 시각 검증은 인앱 브라우저 런타임 오류(Cannot redefine property: process) 때문에 수행하지 못했다. JavaScript/CSS 정적 검증은 통과했다.

## 9. 남은 작업과 주의점

### 1. 실제 OperationFramework 구현

저장소에서 SubscribeReply/SubscribeMonitor의 실제 구현은 찾지 못했다. 웹에 표시하는 계약과 Sample의 raw DDS 수신 예제만 있다.

구현 시 다음을 보장해야 한다.

- ReplyBinding wire 값 → CDM → Semantic Value.name 변환
- fieldName → Semantic Result id 변환
- requestId, controlId, replyId, State 전달
- 다중 Reply와 처리상태 수신
- callback 스레드 및 Dispose 계약 문서화

### 2. Sample IDL/생성 코드 동기화

다음 Sample IDL은 아직 구형 ControlExecutionReply(targetId, executionReport) 구조다.

    Sample/UserTerminal/DdsTypeSupport/OperationManagement.idl
    Sample/OperationManagement/DdsTypeSupport/OperationManagement.idl

web/OperationManagement.idl을 기준으로 Sample IDL을 갱신하고 RTI 생성 코드를 다시 생성해야 한다. 자동 생성 파일을 수동으로 부분 수정하지 말고 프로젝트의 RTI IDL 생성 절차를 먼저 확인한다.

### 3. Value.name 계약 검증

현재 XSD는 Value name을 갖지만, 다음 운영 규칙은 별도 validator 또는 설계 문서로 강화할 수 있다.

- 같은 ValueSet 내 name 중복 금지
- 빈 name 금지
- name 변경을 API breaking change로 취급

사용자가 결정을 바꾸지 않는 한 Value id를 새로 추가하지 않는다.

### 4. 최종 시각 확인

브라우저가 가능한 환경에서 다음을 확인한다.

- Control 표의 긴 명령이 한 줄로 유지되는지
- 복사 버튼이 제목 우측 상단에 붙는지
- 작은 화면에서 표와 버튼이 겹치지 않는지
- Monitor의 HMI 전시 항목 → HMI 모니터 구독 방법 순서

### 5. 병합 상태

현재 최신 변경은 feature/control-profile-identity에 푸시되어 있다. 이 handoff 작성 시점에는 최신 변경을 main에 병합하거나 PR을 만들지 않았다.

## 10. 작업 원칙

- XML 전체 재직렬화보다 최소 diff를 선호한다.
- 기존 주석과 원 프로토콜 Type 철자를 임의로 삭제하거나 수정하지 않는다.
- HMI 화면에 Binding wire 값을 다시 섞지 않는다.
- 실제 wire 값이 필요한 Binding 상세/검증 화면은 유지한다.
- Semantic의 공개 ID/name과 Binding의 실제 숫자/타입을 분리한다.
- 수정 후 대상 XML XSD 검증, node --check web/app.js, git diff --check를 수행한다.
- 커밋 전 git status --short로 사용자 파일이 섞이지 않았는지 확인한다.