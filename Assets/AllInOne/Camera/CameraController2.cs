// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using Unity.Cinemachine;

// public class CameraController2 : MonoBehaviour
// {
//     public static CameraController2 Instance { get; private set; }

//     [SerializeField] private CinemachineCamera cineCamera;
//     [SerializeField] private Camera UIcam;

//     [Space]
//     [SerializeField] private bool useTouch;
//     [SerializeField] private bool useEdgeScrolling = false;
//     [SerializeField] private bool useDragPan = false;

//     [Header("Cursor VFX")]
//     [SerializeField] private GameObject cursorVfx;
//     [SerializeField] private Transform canvas;

//     [Header("Zoom")]
//     [SerializeField] private float fovMin = 15f;
//     [SerializeField] private float fovMax = 50f;
//     [SerializeField] private float followOffsetMin = 10f;
//     [SerializeField] private float followOffsetMax = 40f;

//     [Header("Boundary")]
//     [SerializeField] private Vector2 cameraBoundMin = new Vector2(-20, -20);
//     [SerializeField] private Vector2 cameraBoundMax = new Vector2(20, 20);

//     [Header("Zone")]
//     [SerializeField] private List<Vector3> centerPosList = new List<Vector3>();

//     private List<Vector3> currentCenterPosList = new List<Vector3>();
//     private bool dragAndMoveActive;
//     private bool isDraging;
//     private bool recenter;
//     private Vector2 lastMousePos;
//     private float targetFieldOfView = 10f;
//     private Vector3 followOffSet;
//     private float startTime;
//     private bool flickActive;
//     private bool isCentered = false;
//     private bool centerFound = false;
//     public int currentCenterIndex { get; private set; }
//     private Vector3 inputDir;
//     private Vector2 rawInput;

//     private void Awake()
//     {
//         if (Instance != null && Instance != this) Destroy(this);
//         else Instance = this;

//         followOffSet = cineCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
//     }

//     private void Start()
//     {
//         if (!cineCamera.m_Lens.Orthographic)
//         {

//         }
//         else
//         {
//             if (UIcam != null) UIcam.orthographicSize = cineCamera.m_Lens.OrthographicSize;
//         }

//         currentCenterPosList = centerPosList;
//     }


//     void Update()
//     {
//         if (Time.frameCount < 8) return;

//         AddClickVfx();
//         if (useDragPan) HandleMovementDragPanExtended();

//         //HandleCameraZoom_FOV();
//         RecenterCamera();

//     }

//     public void SetBoundary(Vector2 min, Vector3 max)
//     {
//         cameraBoundMin = min;
//         cameraBoundMax = max;
//     }

//     public void SetAmountOfCenterPos(int amount)
//     {
//         currentCenterPosList = centerPosList.GetRange(0, amount);
//     }

//     private void HandleMovement()
//     {
//         Vector3 inputDir = Vector3.zero;

//         if (Input.GetKey(KeyCode.W)) inputDir.z = +1f;
//         if (Input.GetKey(KeyCode.S)) inputDir.z = -1f;
//         if (Input.GetKey(KeyCode.A)) inputDir.x = -1f;
//         if (Input.GetKey(KeyCode.D)) inputDir.x = +1f;

//         Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;

//         float moveSpeed = 20f;
//         transform.position += moveDir * moveSpeed * Time.deltaTime;
//     }

//     private void AddClickVfx()
//     {
//         if (cursorVfx == null) return;

//         if (Input.GetMouseButtonUp(0) && !isDraging)
//         {
//             if (Utils.IsPointerOverUIElement()) return;
//             GameObject tempVfx = Instantiate(cursorVfx, canvas.transform);
//             tempVfx.GetComponent<RectTransform>().position = Input.mousePosition;
//             tempVfx.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "animation", false);
//             tempVfx.SetActive(true);
//             Destroy(tempVfx, 1f);
//         }
//     }

//     private void HandleMovementEdgeScrolling()
//     {
//         Vector3 inputDir = Vector3.zero;
//         int edgeScrollSize = 20;

//         if (Input.mousePosition.x < edgeScrollSize) inputDir.x = -1f;
//         if (Input.mousePosition.y < edgeScrollSize) inputDir.z = -1f;
//         if (Input.mousePosition.x > Screen.width - edgeScrollSize) inputDir.x = +1f;
//         if (Input.mousePosition.x > Screen.height - edgeScrollSize) inputDir.z = +1f;

//         Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;

//         float moveSpeed = 20f;
//         transform.position += moveDir * moveSpeed * Time.deltaTime;
//     }

//     private void HandleMovementDragPan()
//     {
//         Vector3 inputDir = Vector3.zero;

//         if (!useTouch)
//         {
//             if (Input.GetMouseButtonDown(1))
//             {
//                 dragAndMoveActive = true;
//                 lastMousePos = Input.mousePosition;
//             }

//             if (Input.GetMouseButtonUp(1))
//             {
//                 dragAndMoveActive = false;
//             }

//             if (dragAndMoveActive)
//             {
//                 Vector2 mouseMovementDelta = (Vector2)Input.mousePosition - lastMousePos;
//                 float dragPanSpeed = 20f;

//                 inputDir.x = -mouseMovementDelta.x * dragPanSpeed * Time.deltaTime;
//                 inputDir.z = -mouseMovementDelta.y * dragPanSpeed * Time.deltaTime;
//                 lastMousePos = Input.mousePosition;
//             }
//         }
//         else
//         {
//             if (Input.touchCount != 1 || Utils.IsOverUI()) return;

//             Touch touch = Input.GetTouch(0);

//             if (touch.phase == TouchPhase.Began)
//             {
//                 dragAndMoveActive = true;
//                 lastMousePos = touch.position;

//             }

//             if (touch.phase == TouchPhase.Ended)
//             {
//                 dragAndMoveActive = false;
//             }

//             if (dragAndMoveActive)
//             {
//                 Vector2 mouseMovementDelta = touch.position - lastMousePos;
//                 float dragPanSpeed = 1.5f;

//                 inputDir.x = -mouseMovementDelta.x * dragPanSpeed * Time.deltaTime;
//                 inputDir.z = -mouseMovementDelta.y * dragPanSpeed * Time.deltaTime;

//                 lastMousePos = touch.position;

//             }
//         }

//         Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;

//         float moveSpeed = 10f;
//         Vector3 newPos = transform.position + moveDir * Time.deltaTime * moveSpeed;
//         newPos.x = Mathf.Clamp(newPos.x, cameraBoundMin.x, cameraBoundMax.x);
//         newPos.z = Mathf.Clamp(newPos.z, cameraBoundMin.y, cameraBoundMax.y);

//         transform.position = newPos;
//     }

//     // This is specifically for the Lab3 - Carem studio
//     private void HandleMovementDragPanExtended()
//     {
//         if (!useTouch)
//         {

//         }
//         else
//         {
//             if (Input.touchCount != 1) return;

//             Touch touch = Input.GetTouch(0);

//             if (touch.phase == TouchPhase.Began)
//             {
//                 if (Utils.IsPointerOverUIElement()) return;
//                 dragAndMoveActive = true;
//                 lastMousePos = touch.position;
//                 startTime = Time.time;
//             }
//             else if (touch.phase == TouchPhase.Moved)
//             {
//                 isDraging = true;
//             }
//             else if (touch.phase == TouchPhase.Ended)
//             {
//                 dragAndMoveActive = false;
//                 isDraging = false;
//                 isCentered = false;
//                 if (!recenter) centerFound = false;
//                 float swipeTime = Time.time - startTime;
//                 rawInput = (Vector2)Input.mousePosition - lastMousePos;

//                 if (swipeTime < 0.3f && rawInput.magnitude > 100f)
//                 {
//                     flickActive = true;
//                 }

//             }

//             if (dragAndMoveActive)
//             {
//                 Vector2 mouseMovementDelta = touch.position - lastMousePos;
//                 float dragPanSpeed = 0.5f;

//                 inputDir.x = -mouseMovementDelta.x * dragPanSpeed * Time.deltaTime;
//                 inputDir.z = -mouseMovementDelta.y * dragPanSpeed * Time.deltaTime;

//                 Invoke("ResetPos", 0.3f);

//                 Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;

//                 float moveSpeed = 10f;
//                 Vector3 newPos = transform.position + moveDir * Time.deltaTime * moveSpeed;

//                 newPos.x = transform.position.x;
//                 newPos.z = Mathf.Clamp(newPos.z, cameraBoundMin.y, cameraBoundMax.y);

//                 transform.position = newPos;
//             }
//         }

//         if (flickActive)
//         {

//             if (rawInput.x + rawInput.y < 0)
//             {
//                 if (currentCenterIndex + 1 < currentCenterPosList.Count)
//                     currentCenterIndex++;
//                 centerFound = true;
//             }
//             else if (rawInput.x + rawInput.y > 0)
//             {
//                 if (currentCenterIndex - 1 >= 0)
//                     currentCenterIndex--;
//                 centerFound = true;
//             }

//             flickActive = false;
//         }
//     }

//     public void ChangeZone(int index)
//     {
//         currentCenterIndex = index;

//         recenter = true;
//         centerFound = true;
//     }

//     private void ResetPos()
//     {
//         lastMousePos = Input.mousePosition;
//     }


//     private void RecenterCamera()
//     {
//         if (currentCenterPosList.Count == 0 || dragAndMoveActive) return;

//         if (!isCentered || recenter)
//         {

//             float previousClosetDistance = 99999;

//             if (!centerFound)
//             {
//                 foreach (var center in currentCenterPosList)
//                 {
//                     float distance = Vector3.Distance(transform.position, center);
//                     if (distance < previousClosetDistance)
//                     {
//                         centerFound = true;
//                         currentCenterIndex = currentCenterPosList.IndexOf(center);
//                         previousClosetDistance = distance;
//                     }
//                 }
//             }

//             UIManager.Instance.SwitchUI(currentCenterIndex);

//             float moveSpeed = 5f;
//             transform.position = Vector3.Lerp(transform.position, currentCenterPosList[currentCenterIndex], Time.deltaTime * moveSpeed);

//             if (Vector3.Distance(transform.position, currentCenterPosList[currentCenterIndex]) < 0.05f)
//             {
//                 isCentered = true;
//                 centerFound = false;
//                 recenter = false;
//             }
//         }
//     }

//     private void HandleRotation()
//     {
//         float rotateDir = 0f;
//         if (Input.GetKey(KeyCode.Q)) rotateDir = +1f;
//         if (Input.GetKey(KeyCode.E)) rotateDir = -1f;

//         float rotateSped = 50f;
//         transform.eulerAngles += new Vector3(0, rotateDir * rotateSped * Time.deltaTime, 0);
//     }

//     private void HandleCameraZoom_FOV()
//     {
//         if (!useTouch)
//         {
//             if (Input.mouseScrollDelta.y > 0) targetFieldOfView -= 1f;
//             if (Input.mouseScrollDelta.y < 0) targetFieldOfView += 1f;
//         }
//         else
//         {
//             if (Input.touchCount != 2 || Utils.IsOverUI()) return;

//             Touch touch1 = Input.GetTouch(0);
//             Touch touch2 = Input.GetTouch(1);

//             Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;
//             Vector2 touch2PrevPos = touch2.position - touch2.deltaPosition;

//             float prevMagnitude = (touch1PrevPos - touch2PrevPos).magnitude;
//             float currentMagnitude = (touch1.position - touch2.position).magnitude;
//             float different = currentMagnitude - prevMagnitude;

//             targetFieldOfView -= different * 0.02f;
//         }

//         float zoomSpeed = 10f;
//         targetFieldOfView = Mathf.Clamp(targetFieldOfView, fovMin, fovMax);

//         if (!cineCamera.m_Lens.Orthographic)
//         {
//             cineCamera.m_Lens.FieldOfView = Mathf.Lerp(cineCamera.m_Lens.FieldOfView, targetFieldOfView, Time.deltaTime * zoomSpeed);
//         }
//         else
//         {
//             cineCamera.m_Lens.OrthographicSize = Mathf.Lerp(cineCamera.m_Lens.OrthographicSize, targetFieldOfView, Time.deltaTime * zoomSpeed);
//             if (UIcam != null) UIcam.orthographicSize = cineCamera.m_Lens.OrthographicSize;
//         }
//     }

//     private void HandleCameraZoom_MoveForward()
//     {
//         Vector3 zoomDir = followOffSet.normalized;

//         if (Input.mouseScrollDelta.y > 0) followOffSet -= zoomDir;
//         if (Input.mouseScrollDelta.y < 0) followOffSet += zoomDir;

//         if (followOffSet.magnitude < followOffsetMin) followOffSet = zoomDir * followOffsetMin;
//         if (followOffSet.magnitude > followOffsetMax) followOffSet = zoomDir * followOffsetMax;

//         float zoomSpeed = 10f;
//         cineCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset =
//             Vector3.Lerp(cineCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, followOffSet, Time.deltaTime * zoomSpeed);
//     }

//     private void HandleCameraZoom_LowerY()
//     {
//         float zoomAmount = 3f;

//         if (Input.mouseScrollDelta.y > 0) followOffSet.y -= zoomAmount;
//         if (Input.mouseScrollDelta.y < 0) followOffSet.y += zoomAmount;

//         followOffSet.y = Mathf.Clamp(followOffSet.y, followOffsetMin, followOffsetMax); // this min max only applied on Y Axis

//         float zoomSpeed = 10f;
//         cineCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset =
//             Vector3.Lerp(cineCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, followOffSet, Time.deltaTime * zoomSpeed);
//     }
// }