/***********************************************************************
 ************** SPECIAL INTERACTABLE OBJECT: Video Card ****************
 ****   Allow the user to change the video player in a VideoPlayer  ****
 **** https://github.com/Javieral95/Getting_Started_With_UnityWebXR ****
 **********************************************************************/

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VideoTapeBehaviour : TapeBehaviour
{
    [Header("Video's URL"), Tooltip("Example: 'http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4'")]
    public string Url;
}
