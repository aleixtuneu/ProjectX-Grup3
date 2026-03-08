using UnityEngine;

namespace StarterAssets
{
    public class FootstepSounds : MonoBehaviour
    {
        [SerializeField] private float footstepIntervalWalk = 0.5f;
        [SerializeField] private float footstepIntervalSprint = 0.3f;

        private FirstPersonController _controller;
        private StarterAssetsInputs _input;
        private float _footstepTimer;

        private void Start()
        {
            _controller = GetComponent<FirstPersonController>();
            _input = GetComponent<StarterAssetsInputs>();
        }

        private void Update()
        {
            // If the player is moving
            if (_input.move != Vector2.zero && _controller.Grounded)
            {
                float interval = _input.sprint ? footstepIntervalSprint : footstepIntervalWalk;

                _footstepTimer -= Time.deltaTime;

                if (_footstepTimer <= 0f)
                {
                    AudioManager.Instance.Play(
                        _input.sprint ? AudioClips.Sprint : AudioClips.FootSteps,
                        transform.position
                    );
                    _footstepTimer = interval;
                }
            }
            else
            {
                _footstepTimer = 0f;
            }
        }
    }
}
