using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour {

    [System.Serializable]
    public class Cell
    {
        public bool visited;
        public GameObject up;
        public GameObject left;
        public GameObject right;
        public GameObject down;
    }

    public GameObject wall;
    public float wallLength = 1.0f;
    public int rows = 5;
    public int column = 5;
    private Vector3 initialPos;
    private GameObject wallHolder;
    private Cell[] cells;
    private int totalCell;
    public int currentCell = 0;
    private int visitedCell = 0;
    private bool startedBuild = false;
    private int currentNeighbor = 0;
    private List<int> cellStep;
    private int backingUp = 0;

    // Use this for initialization
    void Start () {
        CreateWalls();
	}

    void CreateWalls()
    {
        wallHolder = new GameObject();
        wallHolder.name = "Maze";

        initialPos = new Vector3((-rows / 2) + wallLength / 2, 0.0f, (-column / 2) + wallLength / 2);
        Vector3 currentPos = initialPos;
        GameObject temp;

        //for X axis
        for (int x = 0; x < column; x++)
        {
            for (int y = 0; y <= rows; y++)
            {
                currentPos = new Vector3(initialPos.x + (y * wallLength) - wallLength / 2, 0.0f, initialPos.z + (x * wallLength) - wallLength / 2);
                temp = Instantiate(wall, currentPos, Quaternion.identity) as GameObject;
                temp.transform.parent = wallHolder.transform;
                wallHolder.transform.parent = transform;
            }
        }

        //for Y axis
        for (int x = 0; x <= column; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                currentPos = new Vector3(initialPos.x + (y * wallLength), 0.0f, initialPos.z + (x * wallLength) - wallLength);
                temp = Instantiate(wall, currentPos, Quaternion.Euler(0.0f,90.0f,0.0f)) as GameObject;
                temp.transform.parent = wallHolder.transform;
                wallHolder.transform.parent = transform;
            }
        }

        CreateCells();

    }

    void CreateCells ()
    {
        totalCell = rows * column;
        int horizontalProcess = 0;
        int childProcess = 0;
        int countProcess = 0;

        GameObject[] allWalls;
        int children = wallHolder.transform.childCount;
        allWalls = new GameObject[children];
        cells = new Cell[rows * column];

        //Get all children
        for (int x = 0; x < children; x++)
        {
            allWalls[x] = wallHolder.transform.GetChild(x).gameObject;
        }

        //assign wall to cells
        for (int process = 0; process < cells.Length; process++)
        {
            cells[process] = new Cell();

            cells[process].left = allWalls[horizontalProcess];
            cells[process].down = allWalls[childProcess + (rows + 1) * column];

            if (countProcess == rows)
            {
                horizontalProcess += 2;
                countProcess = 0;
            }
            else
                horizontalProcess++;

            countProcess++;
            childProcess++;

            cells[process].right = allWalls[horizontalProcess];
            cells[process].up = allWalls[(childProcess + (rows + 1) * column) + rows - 1];
        }

        CreateMaze();

    }

    void CreateMaze ()
    {
        if (visitedCell < totalCell)
        {
            if (startedBuild)
            {
                SetNeighbor();
                if (cells[currentNeighbor].visited == false && cells[currentCell].visited == true)
                {
                    BreakWall();
                    cells[currentNeighbor].visited = true;
                    visitedCell++;
                    cellStep.Add(currentCell);
                    currentCell = currentNeighbor;

                    if (cellStep.Count > 0)
                    {
                        backingUp = cellStep.Count - 1;
                    }
                }
            }
            else
            {
                currentCell = Random.Range(0, totalCell);
                cells[currentCell].visited = true;
                visitedCell++;
                startedBuild = true;
            }
            Invoke("CreateMaze", 0.0f);
        }
    }

    void BreakWall()
    {

    }

    void SetNeighbor()
    {
        int length = 0;
        int[] neighbors = new int[4];
        int corner = 0;
        corner = ((currentCell + 1) / rows);
        corner -= 1;
        corner *= rows;
        corner += rows;

        //right
        if (currentCell + 1 < totalCell && (currentCell + 1) != corner)
        {
            if (cells[currentCell + 1].visited == false)
            {
                neighbors[length] = currentCell + 1;
                length++;
            }
        }

        //left
        if (currentCell - 1 >= 0 && currentCell != corner)
        {
            if (cells[currentCell - 1].visited == false)
            {
                neighbors[length] = currentCell - 1;
                length++;
            }
        }

        //up
        if (currentCell + rows < totalCell)
        {
            if (cells[currentCell + rows].visited == false)
            {
                neighbors[length] = currentCell + rows;
                length++;
            }
        }

        //down
        if (currentCell - rows >= 0)
        {
            if (cells[currentCell - rows].visited == false)
            {
                neighbors[length] = currentCell - rows;
                length++;
            }
        }

        if (length != 0)
        {
            int choosenNeighbor = Random.Range(0, length);
            currentNeighbor = neighbors[choosenNeighbor];
        }
        else
        {
            if (backingUp > 0)
            {
                currentCell = cellStep[backingUp];
                backingUp--;
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
