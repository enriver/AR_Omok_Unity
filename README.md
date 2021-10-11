# AR_Omok_Unity
Unity를 활용한 AR 오목 어플리케이션

<완료>
1. UI 구성 (SceneManager 를 통한 화면 이동)
2. AR Scene 구성 (ARRaycastManager, ARPlaneManager, ARCloudPointManager)
3. Indicator 및 오목판 생성 (hitPose[0].postion, hitPose[0].rotation 위치에 touch event가 발생하면 setActive)
4. Local 저장소를 통한 게임 옵션 유지 (PlayerPrefs)
5. Frame 단위 코드 진행 (Coroutine, Invoke 처리)
6. 3D오목 Obj 파일 안드로이드 렌더링 (Shader 수정)
7. 오목판의 교차 지점에 오목알 놓기 (공간벡터 내적 -> Multi Collider 방식 변경)
8. 사운드, 진동 효과 추가
9. 오목 게임룰 추가 (렌주룰)

<예정>
1. 유저별 고유 ID를 통해 네트워크 송수신 처리 준비
2. 진동 발생 시 오브젝트 흔들림 현상
3. 오목AI 추가
