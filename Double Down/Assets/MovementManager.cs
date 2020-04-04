using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementArrows
{
    Up =    KeyCode.W | KeyCode.UpArrow,
    Down =  KeyCode.S | KeyCode.DownArrow,
    Left =  KeyCode.A | KeyCode.LeftArrow,
    Right = KeyCode.D | KeyCode.RightArrow
}

namespace Managers
{
    public class MovementManager : Singleton<MovementManager>
    {
        public bool canMoveChars = false;
        private bool setupMoveChars = false;
        public bool movedVar = false;
        private Vector3 movement = new Vector3();
        public Camera cam = null;

        public float moveDistance = 0.1f;
        private Vector3[] moves = new Vector3[4]
        {
            new Vector3(0, 0, 1),
            new Vector3(0, 0, -1),
            new Vector3(1, 0, 0),
            new Vector3(-1, 0, 0),
        };

        // Start is called before the first frame update
        void Start()
        {
            cam = Camera.main;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (canMoveChars)
            {
                cam.transform.position = Vector3.Lerp(cam.transform.position, TurnManager.Instance.t1[0].transform.position + new Vector3(0, 2.15f, -4.5f), Time.deltaTime * 10);
                MoveChars();
            }

            if (setupMoveChars)
            {
                cam.transform.position = Vector3.Lerp(cam.transform.position, TurnManager.Instance.t1[0].transform.position + new Vector3(0, 2.15f, -4.5f), Time.deltaTime * 10);
                CheckCameraForMovedChars();
            }
        }

        private void MoveChars()
        {
            if (!TurnManager.Instance.t1[0].GetComponent<CharData>().isInCombat)
            {
                Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), Physics.gravity.y * Time.fixedDeltaTime * moveDistance, Input.GetAxisRaw("Vertical"));

                AnimateChars();

                // Moves the character
                TurnManager.Instance.t1[0].GetComponent<CharacterController>().Move(transform.TransformDirection(input * moveDistance * Time.fixedDeltaTime));
            }
        }

        // Animates characters as they move or stand still
        private void AnimateChars()
        {
            // Changes the character's animation as they move
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
                TurnManager.Instance.t1[0].GetComponent<CharAnimator>().PlayAnimations(AnimationClips.Move);
            else
                TurnManager.Instance.t1[0].GetComponent<CharAnimator>().PlayAnimations(AnimationClips.Idle);

            // Determines whether a character's sprite is flipped or unflipped
            if (Input.GetAxisRaw("Horizontal") < 0)
                TurnManager.Instance.t1[0].GetComponent<SpriteRenderer>().flipX = false;
            else if (Input.GetAxisRaw("Horizontal") > 0)
                TurnManager.Instance.t1[0].GetComponent<SpriteRenderer>().flipX = true;
        }

        private void CheckCameraForMovedChars()
        {
            if ((cam.transform.position.x + 0.01f > TurnManager.Instance.t1[0].transform.position.x && cam.transform.position.x - 0.01f < TurnManager.Instance.t1[0].transform.position.x)
                || (cam.transform.position.z + 0.01f > TurnManager.Instance.t1[0].transform.position.z && cam.transform.position.z - 0.01f < TurnManager.Instance.t1[0].transform.position.z))
            {
                TurnManager.Instance.t1[0].GetComponent<CharacterController>().enabled = true;
                canMoveChars = true;
                setupMoveChars = false;
                StartCoroutine(AllowMove());
            }
        }

        public void StartRound()
        {
            movedVar = false;
            setupMoveChars = true;
            TurnManager.Instance.t1[0].GetComponent<CharacterController>().enabled = false;
            TurnManager.Instance.t1[0].GetComponent<CharData>().MoveCharUI(true);
        }

        IEnumerator AllowMove()
        {
            yield return new WaitForSeconds(0.02f);
            movedVar = true;
            yield return null;
        }
    }
}