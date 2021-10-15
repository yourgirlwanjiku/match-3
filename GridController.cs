using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GridController : MonoBehaviour
{
    public enum PieceType
    {
      EMPTY,
      NORMAL,
      BUBBLE,
      COUNT, //THE NUMBER OF PIECETPES THERE IS GOING TO BE
    };

   [System.Serializable]// so that custom struct shows in the inspector
   public struct PiecePrefab 
   {
   public PieceType type ;
   public GameObject prefab ;
   };
    public int xDim ; //x dimension
    public int yDim ; //y dimension
    public float fillTime ;
    
    public PiecePrefab[] piecePrefabs;
    public GameObject backgroundPrefabs; 
    
    private Dictionary< PieceType, GameObject> piecePrefabDict ;

    private GamePiece[,] pieces ;//the comma notation creates a 2d array

    private bool inverse = false ;
    private GamePiece pressedPiece;
    private GamePiece entredPiece;
    // Start is called before the first frame update
    void Start()
    {
      //instantiate dictionary and check if it contails dictionary
      piecePrefabDict = new Dictionary < PieceType , GameObject>() ;  
      for(int i=0; i<piecePrefabs.Length ; i++){
          if (!piecePrefabDict.ContainsKey(piecePrefabs[i].type)){
              piecePrefabDict.Add(piecePrefabs[i].type,piecePrefabs[i].prefab); 
          }
      }
      //instantiate background prefab
      for(int x =0; x< xDim ; x++){
          for(int y =0 ; y< yDim ; y++){
              GameObject background = (GameObject)Instantiate(backgroundPrefabs, GetWorldPosition(x,y),Quaternion.identity);
              //make it a child of parent
              background.transform.parent = transform ;
          }
      }

      //instantiate all game pieces
      pieces= new GamePiece[xDim,yDim];
      for(int x =0; x< xDim ; x++) {
          for(int y =0 ; y< yDim ; y++){
             SpawnNewPiece(x,y, PieceType.EMPTY);
          }
      }

      //call fill

      Destroy(pieces[ 1,4].gameObject);
    SpawnNewPiece(1,4, PieceType.BUBBLE);

    Destroy(pieces[ 2,4].gameObject);
    SpawnNewPiece(2,4, PieceType.BUBBLE);

     Destroy(pieces[ 3,4].gameObject);
    SpawnNewPiece(3,4, PieceType.BUBBLE);

     Destroy(pieces[ 3,4].gameObject);
    SpawnNewPiece(3,4, PieceType.BUBBLE);

     Destroy(pieces[ 5,4].gameObject);
    SpawnNewPiece(5,4, PieceType.BUBBLE);


     Destroy(pieces[ 6,4].gameObject);
    SpawnNewPiece(6,4, PieceType.BUBBLE);

     Destroy(pieces[ 7,4].gameObject);
    SpawnNewPiece(7,4, PieceType.BUBBLE);

     Destroy(pieces[ 4,0].gameObject);
    SpawnNewPiece(4,0 , PieceType.BUBBLE);

    
     StartCoroutine(Fill ());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
   //call filllstep until the board is filled
    public IEnumerator  Fill(){ 
    while (FillStep()){//the while loop calls fillstep unitl its filled if its true
       
       inverse = !inverse; // call inverse after everystep
      
       yield return  new WaitForSeconds(fillTime) ; 
    }
    }  
    //move the pieces  one space
    public bool FillStep(){
        bool movedPiece = false ;
        //if you encountter a piece you need to move
        // loop through all the columns in reverse order(from bottom to top since our top is zero)

        for(int y = yDim-2 ;y>=0; y--){// start at -2 becasue we are moving pieces down and the bottom piece cannot be mocved down
            
            
            for(int loopX = 0 ; loopX < xDim ;loopX++){// loop through all rows
             
             int x =loopX;
             if(inverse){
                 x =xDim -1 - loopX;
             }
           
            GamePiece piece = pieces [x,y] ;// get game piece at current location and check if its movable

             
             if(piece.IsMovable()){
                 GamePiece pieceBelow =  pieces [x, y+1] ;// check piece below the current piece 
                
                 if(pieceBelow.Type== PieceType.EMPTY)// check if its empty
                 {  
                Destroy (pieceBelow.gameObject);//destroy objects not using
                 piece.MovableComponent.Move(x, y+1 ,fillTime);
                 pieces [x, y+1] = piece;
                 SpawnNewPiece(x, y, PieceType.EMPTY);// spawn piece at the empty space 
                 movedPiece = true ;
                  }
                  //move diagonally 
                  else {
                      for (int diag =-1 ; diag <=1; diag++)//loop through the two diagonals
                      {
                        int diagX  = x + diag;
                      if(inverse )
                      {
                       diagX =x - diag;
                      }
                        if (diagX>= 0 && diagX < xDim ){
                             GamePiece diagonalPiece =pieces [diagX,y +1 ];
                             if(diagonalPiece.Type == PieceType.EMPTY){
                                 bool hasPieceAbove =true;

                                 for (int aboveY=y; aboveY >=0; aboveY--){
                                     GamePiece pieceAbove =pieces[diagX, aboveY];
                                    
                                     if(pieceAbove.IsMovable()){
                                            break;
                                        }
                                        else if (!pieceAbove.IsMovable()&& pieceAbove.Type != PieceType.EMPTY){
                                            hasPieceAbove =false;
                                            break;
                                        }
                                 }

                                 if(!hasPieceAbove){
                                     Destroy(diagonalPiece.gameObject);
                                        piece.MovableComponent.Move(diagX, y+1, fillTime);
                                        pieces[diagX, y+1]=piece;
                                        SpawnNewPiece(x,y, PieceType.EMPTY);
                                        movedPiece=true;
                                        break;
                                 }
                             }
                        }

                      } 
                  }
               }
           }
        
        }
        for(int x=0; x < xDim; x++){
           GamePiece pieceBelow = pieces[x,0];//check if its at the top
           if(pieceBelow.Type == PieceType.EMPTY)
           {
               GameObject newPiece = (GameObject)Instantiate(piecePrefabDict [PieceType.NORMAL], GetWorldPosition(x,-1), Quaternion.identity);
               newPiece.transform.parent = transform  ;

               pieces [x,0]=newPiece.GetComponent<GamePiece>();
               pieces [x,0].Init(x,-1 , this, PieceType.NORMAL);
               pieces [x,0].MovableComponent.Move(x,0 , fillTime);
              // pieces [x,0].ColorComponent.SetColor((ColorPiece.ColorSprite)Random.Range(0,pieces[x,0].ColorComponent.numColors));
               pieces [x,0].ColorComponent.SetColor((ColorPiece.ColorType)Random.Range(0, pieces [x,0].ColorComponent.numColors));
               movedPiece=true;
            }
        }

         return movedPiece ;
    }

    //function to get world position
   public Vector2 GetWorldPosition (int x,  int y)
    {
   return new Vector2 (transform.position.x - xDim/2.0f + x,
   transform.position.y + yDim/2.0f - y );
    }

    public GamePiece SpawnNewPiece(int x, int y, PieceType type){
        GameObject newPiece= (GameObject)Instantiate(piecePrefabDict [type], GetWorldPosition(x,y), Quaternion.identity);
        newPiece.transform.parent = transform ;
        
        pieces [x,y]=newPiece.GetComponent<GamePiece>();
        pieces [x,y].Init(x,y, this, type);
        return pieces [x,y] ;

    }

    //check pieces
 public bool  IsAdjacent(GamePiece piece1, GamePiece piece2){

     return(piece1.X == piece2.X &&(int)Mathf.Abs(piece1.Y -piece2.Y)==1 )//returns true if the x coordinate are in the same x coordinate and y within once space of each other
            || (piece1.Y == piece2.Y &&(int)Mathf.Abs(piece1.X -piece2.X)==1 ) ;

 }

 public void  SwapPieces(GamePiece piece1, GamePiece piece2){
     if (piece1.IsMovable() && piece2.IsMovable()){
        
         pieces[piece1.X, piece1.Y]= piece2;
         pieces[piece2.X, piece2.Y]= piece1;

         //check if pieces are swapped
         
       // store in temporary variables

       int piece1X = piece1.X;
       int piece1Y = piece1.Y;

       piece1.MovableComponent.Move(piece2.X, piece2.Y, fillTime);
        piece2.MovableComponent.Move(piece1X, piece1Y, fillTime);

     }
 }

 public void PressPiece( GamePiece piece)
 {

pressedPiece = piece;
 }

 public void EnterPiece( GamePiece piece)
 {
   entredPiece = piece;

 }

 public void ReleasePiece(){
     if(IsAdjacent(pressedPiece, entredPiece)){
         SwapPieces(pressedPiece, entredPiece);
     }
 }

 //match pieces

   public List <GamePiece> GetMatch(GamePiece piece, int newX, int newY){
       if(piece.IsColored()){
           ColorPiece.ColorType color = piece.ColorComponent.Color; 
        List<GamePiece> horizontalPieces = new List<GamePiece>();
        List<GamePiece> verticalPieces = new List<GamePiece>();
        List<GamePiece> matchingPieces = new List<GamePiece>();

       horizontalPieces.Add(piece);
         for(int dir = 0; dir <=1; dir ++) {
             for(int xOffset = 1; xOffset < xDim; xOffset++ ){
                 int x ;
                 if(dir == 0){  //left
                 x = newX - xOffset;
                 }
                 else {
                     x = newX + xOffset ;
                 }

                 if(x <0 ; || x>xDim){
                     break ;
                 }

                 if(pieces[x, newY].IsColored()&& pieces[ x, newY].ColorComponent.Color== color){
                     horizontalPieces.Add(pieces[x, newY]);
                 }else{
                    break ; 
                 }

             }
         }

         if(horizontalPieces.Count >=3){
             for( int i= 0 ; i<horizontalPieces.Count ; i++){
                 matchingPieces.Add(horizontalPieces[i]);
             }
         }

         if(matchingPieces>=3){
              return matchingPieces ;
         }

         //vertical transversal


         
        verticalPieces.Add(piece);
         for(int dir = 0; dir <=1; dir ++) {
             for(int yOffset = 1; yOffset < yDim; yOffset++ ){
                 int y ;
                 if(dir == 0){  //up
                 y = newY - yOffset;
                 }
                 else {// down
                     y = newY + yOffset ;
                 }

                 if(y <0 ; || y>yDim){
                     break ;
                 }

                 if(pieces[newX, y].IsColored()&& pieces[ newX, y].ColorComponent.Color== color){
                     verticalPieces.Add(pieces[newX, y]);
                 }
                 else{
                    break ; 
                 }

             }
         }

         if(verticalPieces.Count >=3){
             for( int i= 0 ; i<verticalPieces.Count ; i++){
                 matchingPieces.Add(verticalPieces[i]);
             }
         }

         if(matchingPieces>=3){
              return matchingPieces ;
         }
       }

       return null ;
   }
}