#pragma strict

function Start () {
    var distances = new float[32];
    // Set up layer 10 to cull at 15 meters distance.
    // All other layers use the far clip plane distance.
    distances[8] = 40;
    distances[9] = 30;
    camera.layerCullDistances = distances;
}