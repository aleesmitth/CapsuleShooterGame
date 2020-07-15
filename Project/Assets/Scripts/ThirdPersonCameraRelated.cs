using System;
using UnityEngine;
using Cinemachine;
using UnityEngine.Serialization;

/*this file shouldnt be in a camera, it should be a game master of some kind*/

public class ThirdPersonCameraRelated : MonoBehaviour {
    public new CinemachineFreeLook camera;
    public CinemachineFreeLook freeLookCamera;
    public Transform transformMainCamera;
    public Transform transformPlayer;
    public Transform transformGun;
    
    private Transform _transformCamera;
    private Transform _transformFreeLookCamera;
    private readonly int FIRST_PRIORITY = 10;
    private readonly int SECOND_PRIORITY = 9;// tiene q ser menor q la primera

    private void Awake() {
        _transformCamera = camera.transform;
        _transformFreeLookCamera = freeLookCamera.transform;
    }

    // Update is called once per frame
    private void Update() {
        HandleMouseClickCamMov();
        HandleKeyboardCamMov(); //not fully working, idk how to get the keyboard to control the cam rotation
        HandleZoom();
        if (CinemachineCore.Instance.IsLive(camera)) {
            UpdateCharacterDirection(_transformCamera);
        }
    }
    /*toda la implementacion podria estar mucho mejor con un diccionario o una lista pero fue*/

    private void HandleZoom() {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f && camera.m_Lens.FieldOfView > 30f) {
            camera.m_Lens.FieldOfView -= 1;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f && camera.m_Lens.FieldOfView < 60f) {
            camera.m_Lens.FieldOfView += 1;
        }

        if (!Input.GetMouseButton(1)) return;
        if (Input.GetAxis("Mouse ScrollWheel") > 0f && freeLookCamera.m_Lens.FieldOfView > 30f) {
            freeLookCamera.m_Lens.FieldOfView -= 1;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f && freeLookCamera.m_Lens.FieldOfView < 60f) {
            freeLookCamera.m_Lens.FieldOfView += 1;
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
        camera.m_YAxisRecentering.m_enabled = recenterBool;
        camera.m_RecenterToTargetHeading.m_enabled = recenterBool;
        freeLookCamera.m_YAxisRecentering.m_enabled = recenterBool;
        freeLookCamera.m_RecenterToTargetHeading.m_enabled = recenterBool;
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
            UpdateCharacterDirection(_transformFreeLookCamera);
        }

        if (Input.GetMouseButton(1) && Input.GetMouseButtonDown(0)) {
            DisableFreeLook();
        }
        
        else if (Input.GetMouseButtonUp(1)) {
            DisableFreeLook();
        }
    }

    private bool RelevantKeyIsPressed() {
        return Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) ||
               Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.Space);
    }

    private void EnableFreeLook() {
        //freeLook.m_YAxisRecentering.m_enabled = true;
        //freeLook.m_RecenterToTargetHeading.m_enabled = true;
        
        /*actualizo el zoom*/
        freeLookCamera.m_Lens.FieldOfView = camera.m_Lens.FieldOfView;
        /*intercambio prioridad*/
        freeLookCamera.m_Priority = FIRST_PRIORITY;
        camera.m_Priority = SECOND_PRIORITY;
        //freeLook.GetRig(1).GetCinemachineComponent<CinemachineTransposer>().m_ZDamping = 5;
    }

    private void DisableFreeLook() {
        /*actualizo el zoom*/
        camera.m_Lens.FieldOfView = freeLookCamera.m_Lens.FieldOfView;
        /*intercambio prioridad*/
        camera.m_Priority = FIRST_PRIORITY;
        freeLookCamera.m_Priority = SECOND_PRIORITY;
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
    private void UpdateCharacterDirection(Transform transformCamera) {
        /*hago direccion en sentido contraria a la camara*/
        var direction = transformPlayer.position - transformCamera.position;
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
            transformGun.rotation = Quaternion.LookRotation(raycastHit.point - transformGun.position);
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
