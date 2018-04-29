using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bird : MonoBehaviour 
{
	public float upForce;					//Upward force of the "flap".
	private bool isDead = false;			//Has the player collided with a wall?

	private Animator anim;					//Reference to the Animator component.
	private Rigidbody2D rb2d;				//Holds a reference to the Rigidbody2D component of the bird.

	Entity entity;
	NeuralNet neuralNet;

    // Raycast masks
    public LayerMask columnMask;
    
    // Input data
    float columnDistanceY;
    float columnDistanceX;

    // Fitness value 
    float distanceTraveled;

    // Collection of columns
    ColumnPool columnPool;
    GameObject closestColumn;

	void Start()
	{
		//Get reference to the Animator component attached to this GameObject.
		anim = GetComponent<Animator> ();
		//Get and store a reference to the Rigidbody2D attached to this GameObject.
		rb2d = GetComponent<Rigidbody2D>();

		entity = GetComponent<Entity> ();
		neuralNet = entity.neuralNet;

        columnDistanceY = 0.0f;

        distanceTraveled = 0.0f;

        columnPool = GameObject.FindObjectOfType<ColumnPool>();
	}

	void FixedUpdate()
	{
        //Don't allow control if the bird has died.
        if (isDead == false) {

            // find closest column to us
            closestColumn = FindClosestColumn();

            // Get input values
            CheckColumnDistanceY();
            CheckColumnDistanceX();

            List<float> inputs = new List<float>();

            // Scale input values
            columnDistanceY = columnDistanceY / 6.0f;
            columnDistanceX = columnDistanceX / 6.0f;

            // Add inputs to list
            inputs.Add(columnDistanceY);
            inputs.Add(columnDistanceX);

            entity.neuralNet.SetInput(inputs);
            entity.neuralNet.refresh();

            float outputVal = entity.neuralNet.GetOutput(0);
            //Debug.Log("OutputValue: " + outputVal);

			//Look for input to trigger a "flap".
			if (outputVal >= 0.5f) {
				//...tell the animator about it and then...
				anim.SetTrigger ("Flap");
				//...zero out the birds current y velocity before...
				rb2d.velocity = Vector2.zero;
				//	new Vector2(rb2d.velocity.x, 0);
				//..giving the bird some upward force.
				rb2d.AddForce (new Vector2 (0, upForce));
			}

            distanceTraveled += Time.deltaTime;
            entity.EntityUpdate(distanceTraveled);
		}
	}


    void CheckColumnDistanceY()
    {
        if (closestColumn != null)
        {
            // Get distance from middle of column to bird
            columnDistanceY = closestColumn.transform.position.y - transform.position.y;

            if(columnDistanceY < 0.0f)
            {
                columnDistanceY = 0.0f;
            }
        }
        else
        {
            columnDistanceY = 1.0f;
        }
    }

    void CheckColumnDistanceX()
    {
        if (closestColumn != null)
        {
            // Get distance from middle of column to bird
            columnDistanceX = closestColumn.transform.position.x - transform.position.x;

            if (columnDistanceX > 6.0f)
            {
                columnDistanceX = 6.0f;
            }
        }
        else
        {
            columnDistanceX = 1.0f;
        }
    }

    GameObject FindClosestColumn()
    {
        GameObject closestColumn = null;
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < columnPool.columns.Length; i++)
        {
            GameObject column = columnPool.columns[i];
            float distanceToColumn = Vector2.Distance(transform.position, column.transform.position);

            // If our distance to column is less than closestDistance
            // AND our x position is less than columns' x position
            if(distanceToColumn < closestDistance && 
               transform.position.x < (column.transform.position.x + 1.0f)) // Add 1 to make sure the bird passed the column
            {
                // We have our current closest column
                closestColumn = column;
                closestDistance = distanceToColumn;
            }
        }

        return closestColumn;
    }


    void OnCollisionEnter2D(Collision2D other)
	{
        //if (other.gameObject.tag != "Roof") {
            // Bird died so we need new generation
            entity.EntityFailed();
            // Reset distance on death
            distanceTraveled = 0.0f;
			// Zero out the bird's velocity
			rb2d.velocity = Vector2.zero;
			// If the bird collides with something set it to dead...
			isDead = true;
			//...tell the Animator about it...
			//	anim.SetTrigger ("Die");
			//...and tell the game control about it.
			GameControl.instance.BirdDied ();
		//}
	}

	public void Reset()
	{
        //transform.rotation = Quaternion.identity;
        transform.position = new Vector3 (-1.8f, 0.5f, 0f);
		rb2d.velocity = Vector2.zero;
		rb2d.angularVelocity = 0.0f;
		isDead = false;
	}
}
