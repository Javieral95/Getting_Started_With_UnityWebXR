using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapeLectorBehaviour : MonoBehaviour
{
    [Header("Tape Lector Settings")]
    [Tooltip("The place where the Video Card will place")]
    public Transform PlayerCardPosition;
    public MultimediaType TapeType;

    //Video Card Options
    protected TapeBehaviour tape;
    protected bool _tapeInside;
    protected int _tapeId;

    // Events
    protected bool CheckEnteredTape(Collider other)
    {
        if (!_tapeInside && other.CompareTag(Constants.INTERACTABLE_TAG))
        {
            _tapeInside = other.gameObject.TryGetComponent(out tape);

            if (TapeType == MultimediaType.Video)            
                _tapeInside = tape.GetType().IsAssignableFrom(typeof(VideoTapeBehaviour));            
            else if (TapeType == MultimediaType.Audio)            
                _tapeInside = tape.GetType().IsAssignableFrom(typeof(MusicTheoryTapeBehaviour));            
            else
                _tapeInside = false;

            if (_tapeInside)
            {
                _tapeId = other.GetInstanceID();
                //Move object to correct transform and block it
                //ScreenVideo.ChangeVideoClip(_tapeId.Url);
                tape.PutOnLector(PlayerCardPosition);
                return true;
            }

        }
        return false;
    }

    protected bool CheckExitTape(Collider other)
    {
        if (_tapeInside && other.CompareTag(Constants.INTERACTABLE_TAG) && _tapeId == other.GetInstanceID())
        {
            tape.PutOffLector();
            _tapeInside = false;
            tape = null;
            return true;
            //ScreenVideo.ChangeVideoClip(string.Empty);
        }
        return false;
    }
}

public enum MultimediaType
{
    Audio,
    Video,
    Photo
}
