using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementArrows
{
    Up = KeyCode.W | KeyCode.UpArrow,
    Down = KeyCode.S | KeyCode.DownArrow,
    Left = KeyCode.A | KeyCode.LeftArrow,
    Right = KeyCode.D | KeyCode.RightArrow
}

namespace Managers
{
    public class MovementManager : Singleton<MovementManager>
    {
        public List<CharData> playerChars;
        public bool canMoveChars = true;
        private Vector3 movement = new Vector3();

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

        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (canMoveChars)
                MoveChars();
        }

        private void MoveChars()
        {
            Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), Physics.gravity.y * Time.fixedDeltaTime * moveDistance, Input.GetAxisRaw("Vertical"));
            playerChars[0].gameObject.GetComponent<CharacterController>().Move(transform.TransformDirection(input * moveDistance * Time.fixedDeltaTime));
        }
    }
}