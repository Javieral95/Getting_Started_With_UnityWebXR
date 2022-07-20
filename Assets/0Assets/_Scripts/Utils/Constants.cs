using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    // Tags
    public static readonly string INTERACTABLE_TAG = "Interactable";
    public static readonly string INTERACTABLE_NOT_MOVABLE_TAG = "InteractableNotMovable";

    public static readonly string PLAYER_TAG = "Player";
    public static readonly string HANDS_TAG = "PlayerHands";
    public static readonly string SCREEN_OBJECT_TAG = "ScreenObject";

    // For animators
    public static readonly string IS_ACTIVATE = "isActivate";
    public static readonly string IS_RIGHT_ANIMATION = "moveRight";
    public static readonly string IS_LEFT_ANIMATION = "moveLeft";

    //MinIO
    public static readonly string GET_BUCKETS_ENDPOINT = "/buckets";
    public static readonly string CHECK_BUCKET_ENDPOINT = "/check_bucket";
    public static readonly string GET_BUCKET_OBJECTS_ENDPOINT = "/get_objects";

    // Default names
    public static readonly string TEST_VIDEO_URL = "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4";
    public static readonly string DEFAULT_VIDEO_FILE_NAME = "video1.mp4";
    public static readonly string VIDEO_SERVER_SETTINGS_FILE_NAME = "videoServerConfig.json";
}
