using System;
using System.Collections;
using UnityEngine;
using Cinemachine;
using UnityEngine.Serialization;

/*this file shouldnt be in a camera, it should be a game master of some kind*/

public class CameraMovementAndStuff : MonoBehaviour {
    public CinemachineFreeLook fixedThirdPersonCamera;
    public CinemachineFreeLook freeLookThirdPersonCamera;
    public CinemachineVirtualCamera firstPersonCamera;
    public Transform transformMainCamera;
    public Transform transformPlayer;
    public Transform transformGun;
    
    private Transform _transformThirdPersonCamera;
    private Transform _transformFreeLookCamera;
    private Transform _transformFirstPersonCamera;
    private readonly int FIRST_PRIORITY = 1;
    private readonly int NO_PRIORITY = 0;// tiene q ser menor q la primera

    private void Awake() {
        _transformThirdPersonCamera = fixedThirdPersonCamera.transform;
        _transformFreeLookCamera = freeLookThirdPersonCamera.transform;
        _transformFirstPersonCamera = firstPersonCamera.transform;
    }

    // Update is called once per frame
    private void Update() {
        VerifyTypeOfCameraRequested();
        if (!CinemachineCore.Instance.IsLive(firstPersonCamera)) {
            HandleMouseClickCamMov();
            HandleKeyboardCamMov(); //not fully working, idk how to get the keyboard to control the cam rotation
            HandleZoom();
            if (CinemachineCore.Instance.IsLive(fixedThirdPersonCamera)) {
                UpdateCharacterDirection(_transformThirdPersonCamera.position);
            }
        }
        else {
            HandleMouseMovementFirstPerson();
        }




    }
    // hay un bug que me molesta, hace que cuando cambio de first a third person, como el arma esta
    // adelante del player, la camara de third se pone del lado de adelante del player, y me invierte a donde
    // estaba mirando el player cuando estaba en first
    
    // lo arregle y no se como... creo q distintas referencias de transform, o gun NI IDEA.
    private void VerifyTypeOfCameraRequested() {
        // si toco la c y no esta ni la first person ni la fixed third person activas, returnea (esto es para q no me
        // rompa tod.o cuando toco la c y estoy en freelook)
        
        if ((!Input.GetKeyUp(KeyCode.C) || !CinemachineCore.Instance.IsLive(firstPersonCamera)) &&
            (!Input.GetKeyUp(KeyCode.C) || !CinemachineCore.Instance.IsLive(fixedThirdPersonCamera))) return;
        var aux = fixedThirdPersonCamera.m_Priority;
        fixedThirdPersonCamera.m_Priority = firstPersonCamera.m_Priority;
        firstPersonCamera.m_Priority = aux;
        centrarCamara();
    }

    private void centrarCamara() {
        ModificarRecentering(true);
        // forma villera de recentrar mi camara, a veces anda a veces no porque el tiempo para el fixed update es muy corto
        // es horrible esto, pero cinemachine recenter esta bugeado.
        StartCoroutine(WaitOneSecondAndDeactivateRecentering());
    }

    private IEnumerator WaitOneSecondAndDeactivateRecentering() {
        yield return new WaitForFixedUpdate();
        ModificarRecentering(false);
    }

    private void HandleMouseMovementFirstPerson() {
        UpdateCharacterDirectionFirstPerson();
    }

    private void UpdateCharacterDirectionFirstPerson() {
        // do something
        if (!Physics.Raycast(transformMainCamera.position, transformMainCamera.forward, out var raycastHit)) return;
        
        var directionPlayer = raycastHit.point - transformPlayer.position;
        var gunDirection = directionPlayer;
        directionPlayer.y = 0;
        transformPlayer.rotation = Quaternion.LookRotation(directionPlayer, Vector3.up);
        UpdateGunDirectionFirstPerson();
    }

    private void UpdateGunDirectionFirstPerson() {
        if (Physics.Raycast(transformMainCamera.position, transformMainCamera.forward, out var raycastHit)) {
            transformGun.rotation = Quaternion.LookRotation(raycastHit.point - transformGun.position, Vector3.up);
        }
    }
    /*toda la implementacion podria estar mucho mejor con un diccionario o una lista pero fue*/

    private void HandleZoom() {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f && fixedThirdPersonCamera.m_Lens.FieldOfView > 30f) {
            fixedThirdPersonCamera.m_Lens.FieldOfView -= 1;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f && fixedThirdPersonCamera.m_Lens.FieldOfView < 60f) {
            fixedThirdPersonCamera.m_Lens.FieldOfView += 1;
        }

        if (!Input.GetMouseButton(1)) return;
        if (Input.GetAxis("Mouse ScrollWheel") > 0f && freeLookThirdPersonCamera.m_Lens.FieldOfView > 30f) {
            freeLookThirdPersonCamera.m_Lens.FieldOfView -= 1;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f && freeLookThirdPersonCamera.m_Lens.FieldOfView < 60f) {
            freeLookThirdPersonCamera.m_Lens.FieldOfView += 1;
        }
    }
    
    private void HandleKeyboardCamMov() {
        /*if (Input.GetKeyDown(KeyCode.Q)) {
            RotateCameraLeft();
        }

        if (Input.GetKeyDown(KeyCode.E)) {
            RotateCameraRight();
        }*/
        if (Input.GetKeyDown(KeyCode.R)) {
            ModificarRecentering(true);
        }

        if (Input.GetKeyUp(KeyCode.R)) {
            ModificarRecentering(false);
        }
    }

    private void ModificarRecentering(bool recenterBool) {
        fixedThirdPersonCamera.m_YAxisRecentering.m_enabled = recenterBool;
        fixedThirdPersonCamera.m_RecenterToTargetHeading.m_enabled = recenterBool;
        freeLookThirdPersonCamera.m_YAxisRecentering.m_enabled = recenterBool;
        freeLookThirdPersonCamera.m_RecenterToTargetHeading.m_enabled = recenterBool;
    }

    private void RotateCameraRight() {

    }

    private void RotateCameraLeft() {
        
        //freeLook.m_XAxis.m_InputAxisName = "Q"; quiero girar camara con Q y E
    }

    private void HandleMouseClickCamMov() {
        if (Input.GetMouseButtonDown(1)) {
            EnableFreeLook();
        }

        if (Input.GetMouseButton(1) && RelevantKeyIsPressed()) {
            UpdateCharacterDirection(_transformFreeLookCamera.position);
        }

        if (Input.GetMouseButton(1) && Input.GetMouseButtonDown(0)) {
            DisableFreeLook();
        }
        
        else if (Input.GetMouseButtonUp(1)) {
            DisableFreeLook();
        }
    }

    private bool RelevantKeyIsPressed() {
        return Input.GetKey(KeyCode.W) /*|| Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) ||       solo reseteo la direccion del player si toca la W o dispara
               Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.Space)*/;
    }

    private void EnableFreeLook() {
        //freeLook.m_YAxisRecentering.m_enabled = true;
        //freeLook.m_RecenterToTargetHeading.m_enabled = true;
        
        /*actualizo el zoom*/
        freeLookThirdPersonCamera.m_Lens.FieldOfView = fixedThirdPersonCamera.m_Lens.FieldOfView;
        /*intercambio prioridad*/
        freeLookThirdPersonCamera.m_Priority = FIRST_PRIORITY;
        fixedThirdPersonCamera.m_Priority = NO_PRIORITY;
        //freeLook.GetRig(1).GetCinemachineComponent<CinemachineTransposer>().m_ZDamping = 5;
    }

    private void DisableFreeLook() {
        /*actualizo el zoom*/
        fixedThirdPersonCamera.m_Lens.FieldOfView = freeLookThirdPersonCamera.m_Lens.FieldOfView;
        /*intercambio prioridad*/
        fixedThirdPersonCamera.m_Priority = FIRST_PRIORITY;
        freeLookThirdPersonCamera.m_Priority = NO_PRIORITY;
    }
    
    
    
    
    
    /* modifico un par d settings del cinemachine*/

    /*private void DisableFreeLook() {
        //freeLook.m_YAxisRecentering.m_enabled = true;
        //freeLook.m_RecenterToTargetHeading.m_enabled = true;
        freeLookCamera.m_YAxis.m_InputAxisName = "";
        freeLookCamera.m_XAxis.m_InputAxisName = "";
        freeLookCamera.m_XAxis.m_MaxSpeed = 0;
        freeLookCamera.m_YAxis.m_MaxSpeed = 0;
        //freeLook.GetRig(1).GetCinemachineComponent<CinemachineTransposer>().m_ZDamping = 5;
    }

    private void EnableFreeLook() {
        freeLookCamera.m_YAxisRecentering.m_enabled = false;
        freeLookCamera.m_RecenterToTargetHeading.m_enabled = false;
        freeLookCamera.m_YAxis.m_InputAxisName = "Mouse Y";
        freeLookCamera.m_XAxis.m_InputAxisName = "Mouse X";
        freeLookCamera.m_XAxis.m_MaxSpeed = 450;
        freeLookCamera.m_YAxis.m_MaxSpeed = 6;
        //freeLook.GetRig(1).GetCinemachineComponent<CinemachineTransposer>().m_ZDamping = 0;
    }*/

    /*lo interesate: resta de vectores, posCam y posPlayer, el resultado es a donde tiene q mirar el jugador (SOLO EJES X, Z)*/
    private void UpdateCharacterDirection(Vector3 positionCamera) {
        /*hago direccion en sentido contraria a la camara*/
        var direction = transformPlayer.position - positionCamera;
        direction.y = 0;
        /*transformo direccion en rotacion*/
        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
        /*roto al jugador*/
        transformPlayer.rotation = rotation;
        UpdateGunDirection();

    }
    /*deje varias funciones comentadas q eran otras formas de hacerlo q no salieron, no se si porque estaban mal o yo tenia bugs en otro lado
     (habia un rigidbody encima del arma, estuve como 5horas con eso)*/
    /* hay un gran problema q es el sharp movement cuando apunto a un objeto y despues a otro pero bue*/
    private void UpdateGunDirection() {
        //Vector3 mousePosition = Input.mousePosition;
        //var direction = transformGun.position - transformFreeLookCamera.position;
        //var direction = freeLookCamera.transform.forward - transformFreeLookCamera.position;
        var positionMainCamera = transformMainCamera.position;
        // var direction = new Vector3(0,0, positionMainCamera.z);
        // if (Physics.Raycast(positionMainCamera, direction, out var raycastHitInfo)) {
        //     direction = transformGun.position - raycastHitInfo.transform.position;
        //     transformGun.rotation = Quaternion.LookRotation(direction, Vector3.up);
        // }

        /*queria setear en vez de a cuales pegarles, a cuales ignorar pero se ve q es complicado.. shame*/
        int layerMask = LayerMask.GetMask("Enviroment");
        /* aca lo importante a saber es q si le paso un float lo toma como distancia, el int es la layer
            el raycast hace varias cosas, puedo ver los metodos de clase y generalmente es tirar un laser de un origen en
            una direccion y si choca con algo toda la info del objeto la guarda en RayCastHit, y devuelve true*/
        // si quiero ignorar los triggers
        // Raycast(positionMainCamera, transformMainCamera.forward, out var raycastHit, Mathf.Infinity, layerMask , QueryTriggerInteraction.Ignore)
        // tambien funciona sin Mathf y con este termino, pero es mas costoso && !raycastHit.collider.isTrigger
        // y probar que pasa si pongo un objeto abajo del trigger, o no.
        
        //importante: el raycast tiene por default una distancia de 250 maso menos aunque le ponga el mathf infinity
        if (Physics.Raycast(positionMainCamera, transformMainCamera.forward, out var raycastHit, layerMask) &&
            NoEsUnBugConLaCamara(raycastHit)) {
            /*muy importante usar point y no position porque me devuelve el centro del objeto que golpeo*/
            transformGun.rotation = Quaternion.LookRotation(raycastHit.point - transformGun.position, Vector3.up);
        }

        /*
        //transformo direccion en rotacion
        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
        //roto al jugador
        transformGun.rotation = rotation;*/
    }

    /*metodo bastante hardcodeado, es para que si estando en 3rd persona, mi cursor le apunta a un objeto que
     esta atras de mi jugador, el jugador no se rompa los brazos y dispare para atras, el forward veo si tiene el mismo signo que la direccion del objeto, si la tienen actualizo la direccion del arma*/
    private bool NoEsUnBugConLaCamara(RaycastHit raycastHit) {
        /* devuelve un valor entre 0 y 1, compara la normas algo asi, no lo investigue mucho, si es >0 el primer termino esta adelante del segundo*/
        return Vector3.Dot(raycastHit.point.normalized, transformPlayer.forward.normalized) > 0;
    }
}
