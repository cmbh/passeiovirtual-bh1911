#pragma strict

public var scrollSpeed:float = 0.25;

public function FixedUpdate() : void
{
    var offset:float = Time.time * scrollSpeed;
    renderer.material.mainTextureOffset = Vector2(offset,offset);
}