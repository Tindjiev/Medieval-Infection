using UnityEngine;

public class WalkSound : MonoBehaviour
{
    [SerializeField]
    private AudioSource _rightFootAudio, _leftFootAudio;

    [SerializeField]
    private SoundsSO _sounds;

    private AudioClip[] _footstepsSounds => _sounds.sounds;

    private void Awake()
    {
        WalkAnimation temp = GetComponent<WalkAnimation>();
        temp.RightFootStep.AddListener(() => PlayFootstepAudio(_rightFootAudio));
        temp.LeftFootStep.AddListener(() => PlayFootstepAudio(_leftFootAudio));
    }

    private void PlayFootstepAudio(AudioSource rightOrLeft)
    {
        rightOrLeft.clip = _footstepsSounds.GetRandomElement();
        rightOrLeft.Play();
    }



}
