using UnityEngine;
using System.Collections.Generic;

namespace UnityInterwork {

class EntityMirror: MonoBehaviour {
    public Game.Entity entity;
    public int team;
    public float angle;

    public GameObject healthBarPrefab = null;
    public GameObject resourceBarPrefab = null;

    Testshit testshit = null;

    int currentTeamColour = -1;

    // For interpolation.
    float interpolationTime;
    int nextTick;
    Vector3 currentPosition;
    Vector3 nextPosition;
    Vector3 currentRotation;
    Vector3 nextRotation;

    void UpdateTeamColour() {
        if(currentTeamColour != entity.team) {
            foreach(var mr in GetComponentsInChildren<Renderer>()) {
                mr.material.SetColor("_TeamColor", testshit.TeamColour(entity.team));
            }
            currentTeamColour = entity.team;
        }
    }

    void Start() {
        if(entity == null) {
            return;
        }

        testshit = Object.FindObjectOfType<Testshit>();

        foreach(var comp in entity.components) {
            if(comp is Game.Factory) {
                var m = gameObject.AddComponent<FactoryMirror>();
                m.component = (Game.Factory)comp;
            } else if(comp is Game.ResourceSource) {
                var m = gameObject.AddComponent<ResourceSourceMirror>();
                m.component = (Game.ResourceSource)comp;
            } else if(comp is Game.ResourcePool) {
                var m = gameObject.AddComponent<ResourcePoolMirror>();
                m.component = (Game.ResourcePool)comp;
            } else if(comp is Game.Wizard) {
                var m = gameObject.AddComponent<WizardMirror>();
                m.component = (Game.Wizard)comp;
            } else if(comp is Game.Truck) {
                var m = gameObject.AddComponent<TruckMirror>();
                m.component = (Game.Truck)comp;
            } else if(comp is Game.Collider) {
                var m = gameObject.AddComponent<ColliderMirror>();
                m.component = (Game.Collider)comp;
            } else if(comp is Game.ProjectileWeapon) {
                var m = gameObject.AddComponent<ProjectileWeaponMirror>();
                m.component = (Game.ProjectileWeapon)comp;
            } else if(comp is Game.HitscanWeapon) {
                var m = gameObject.AddComponent<HitscanWeaponMirror>();
                m.component = (Game.HitscanWeapon)comp;
            } else if(comp is Game.Health) {
                var m = gameObject.AddComponent<HealthMirror>();
                m.healthBarPrefab = healthBarPrefab;
                m.canvasTransform = Object.FindObjectOfType<PlayerInterface>().screenCanvas;
                m.component = (Game.Health)comp;
            } else if(comp is Game.ResourceHarvester) {
                var m = gameObject.AddComponent<ResourceHarvesterMirror>();
                m.resourceBarPrefab = resourceBarPrefab;
                m.canvasTransform = Object.FindObjectOfType<PlayerInterface>().screenCanvas;
                m.component = (Game.ResourceHarvester)comp;
            } else if(comp is Game.PartialBuilding) {
                var m = gameObject.AddComponent<PartialBuildingMirror>();
                m.resourceBarPrefab = resourceBarPrefab;
                m.canvasTransform = Object.FindObjectOfType<PlayerInterface>().screenCanvas;
                m.component = (Game.PartialBuilding)comp;
            } else if(comp is Game.WizardTower) {
                var m = gameObject.AddComponent<WizardTowerMirror>();
                m.component = (Game.WizardTower)comp;
            } else if(comp is Game.BuildRadius) {
                var m = gameObject.AddComponent<BuildRadiusMirror>();
                m.component = (Game.BuildRadius)comp;
            } else {
                Logger.Log("Unmirrorable component {0}", comp);
            }
        }
        UpdateTeamColour();
        transform.position = (Vector3)entity.position;
        transform.eulerAngles = new Vector3(0, (float)entity.rotation, 0);

        interpolationTime = 0;
        nextTick = Game.World.current.currentTick;
        currentPosition = transform.position;
        nextPosition = currentPosition;
        currentRotation = new Vector3(0, (float)entity.rotation, 0);
        nextRotation = currentRotation;
    }

    void Update() {
        if(entity == null) {
            return;
        }
        UpdateTeamColour();

        interpolationTime += Time.deltaTime;
        if(interpolationTime > (float)Game.World.deltaTime) {
            interpolationTime = (float)Game.World.deltaTime;
        }

        if(nextTick != Game.World.current.currentTick) {
            interpolationTime = 0;
            currentPosition = nextPosition;
            currentRotation = nextRotation;
            nextPosition = (Vector3)entity.position;
            nextRotation = new Vector3(0, (float)entity.rotation, 0);
            nextTick = Game.World.current.currentTick;
        }

        var lerp = interpolationTime / (float)Game.World.deltaTime;
        transform.position = Vector3.Lerp(currentPosition, nextPosition, lerp);
        transform.eulerAngles = Vector3.Lerp(currentRotation, nextRotation, lerp);

        if(!testshit.enableInterpolation) {
            transform.position = nextPosition;
            transform.eulerAngles = nextRotation;
        }

        team = entity.team;
        angle = (float)entity.rotation;
    }

    public void Destroyed() {
        Destroy(gameObject);
    }
}

}
