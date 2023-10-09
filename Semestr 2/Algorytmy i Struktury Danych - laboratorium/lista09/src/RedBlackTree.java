public class RedBlackTree{
    public static final String ANSI_RESET = "\u001B[0m";
    public static final String ANSI_RED = "\u001B[31m";

    private class Node{
        private final int key;
        private Node parent;
        private Node left;
        private Node right;
        private boolean isRed;
        private int leftNodes;
        private int rightNodes;
        private int leftHeight;
        private int rightHeight;

        public Node(int key){
            this.key = key;
            this.parent=null;
            this.left=null;
            this.right=null;
            this.isRed=true;
        }
    }

    private Node root;

    public RedBlackTree(){
        this.root=null;
    }

    private void rotateLeft(Node oldRoot){
        Node newRoot = oldRoot.right;
        Node oldLeft = newRoot.left;

        newRoot.left = oldRoot;
        oldRoot.right = oldLeft;

        if (oldRoot.parent!=null){
            if (oldRoot.key<oldRoot.parent.key){
                oldRoot.parent.left=newRoot;
            }else{
                oldRoot.parent.right=newRoot;
            }

        }else{
            this.root=newRoot;
        }
        newRoot.parent=oldRoot.parent;
        oldRoot.parent = newRoot;
        if (oldLeft!=null){
            oldLeft.parent = oldRoot;
        }
    }

    private void rotateRight(Node oldRoot){
        Node newRoot = oldRoot.left;
        Node oldRight = newRoot.right;

        newRoot.right = oldRoot;
        oldRoot.left = oldRight;

        if (oldRoot.parent!=null){
            if (oldRoot.key<oldRoot.parent.key){
                oldRoot.parent.left=newRoot;
            }else{
                oldRoot.parent.right=newRoot;
            }

        }else{
            this.root=newRoot;
        }
        newRoot.parent=oldRoot.parent;
        oldRoot.parent = newRoot;
        if (oldRight!=null) {
            oldRight.parent = oldRoot;
        }
    }

    public void insert(int key){
        Node newNode = new Node(key);
        Node prev = null;
        Node temp = this.root;

        while (temp!=null){
            prev=temp;
            if (key>temp.key){
                temp=temp.right;
            }else{
                temp=temp.left;
            }
        }

        if (prev==null){
            this.root=newNode;
        }else if(key>prev.key){
            newNode.parent=prev;
            prev.right=newNode;
        }else{
            newNode.parent=prev;
            prev.left=newNode;
        }
        insertFix(newNode);
    }
    private void insertFix(Node node){
        if (this.root==node){
            //if x is root
            node.isRed=false;
        }else if (node.parent.isRed){
            //double red case
            Node grandparent=node.parent.parent;
            Node uncle;
            if (node.key>grandparent.key){
                uncle=node.parent.parent.left;
            }else{
                uncle=node.parent.parent.right;
            }

            if (uncle!=null && uncle.isRed){
                //uncle is red
                uncle.isRed=false;
                node.parent.isRed=false;
                grandparent.isRed=true;
                insertFix(grandparent);
            }else{
                //uncle is black
                if (node.parent==grandparent.left && node==node.parent.left){
                    //left left case
                    rotateRight(grandparent);
                    swapColors(grandparent, node.parent);
                }else if (node.parent==grandparent.left && node==node.parent.right){
                    //left right case
                    rotateLeft(node.parent);

                    rotateRight(grandparent);

                    swapColors(grandparent, node);
                }else if (node.parent==grandparent.right && node==node.parent.right){
                    //right right case
                    rotateLeft(grandparent);
                    swapColors(grandparent, node.parent);
                }else if (node.parent==grandparent.right && node==node.parent.left){
                    //right left case
                    rotateRight(node.parent);

                    rotateLeft(grandparent);

                    swapColors(grandparent, node);
                }
            }
        }
    }
    private void swapColors(Node n1, Node n2){
        boolean temp = n1.isRed;
        n1.isRed=n2.isRed;
        n2.isRed=temp;
    }


    private void inOrderTraversal(Node node){
        if (node!=null){
            inOrderTraversal(node.left);
            if (!node.isRed) System.out.print(node.key+" ");
            else System.out.print(ANSI_RED+node.key+" "+ANSI_RESET);
            inOrderTraversal(node.right);
        }
    }
    public void inOrderTraversal(){
        inOrderTraversal(root);
    }

    private void printCurrentLevel(Node root, int level){
        if (root == null)
            return;
        if (level == 1) {
            if (!root.isRed) System.out.print(root.key + " ");
            else System.out.print(ANSI_RED + root.key + " " + ANSI_RESET);
        }
        else if (level > 1) {
            printCurrentLevel(root.left, level - 1);
            printCurrentLevel(root.right, level - 1);
        }
    }
    public void levelOrderTraversal(){
        int h = height(root);
        int i;
        for (i = 1; i <= h; i++) {
            printCurrentLevel(root, i);
            System.out.print("|");
        }
    }

    public boolean search(int key){
        Node node=root;
        while(node!=null && node.key!=key){
            if (key>node.key)node=node.right;
            else node=node.left;
        }
        return node!=null;
    }

    private int height(Node root){
        if (root == null)
            return 0;
        else {
            int lheight = height(root.left);
            int rheight = height(root.right);

            if (lheight > rheight)
                return (lheight + 1);
            else
                return (rheight + 1);
        }
    }
    public int height(){return height(root);}

    private int rightHeight(Node node){
        if (node==null)return 0;
        return height(node.right);
    }
    public int rightHeight(){return rightHeight(root);}

    private int leftHeight(Node node){
        if (node==null)return 0;
        return height(node.left);
    }
    public int leftHeight(){return leftHeight(root);}

    private int nodesCount(Node node){
        if (node!=null) {
            int count = 1;
            count += nodesCount(node.left);
            count += nodesCount(node.right);
            return count;
        }
        return 0;
    }
    public int nodesCount(){
        return nodesCount(root);
    }

    private int rightNodesCount(Node node){
        if (node==null)return 0;
        return nodesCount(node.right);
    }
    public int rightNodesCount(){
        return rightNodesCount(root);
    }

    private int leftNodesCount(Node node){
        if (node==null)return 0;
        return nodesCount(node.left);
    }
    public int leftNodesCount(){
        return leftNodesCount(root);
    }

    public void getInfo(){
        if (this.root!=null) {
            System.out.println("wysokość drzewa: " + (height() - 1));
            System.out.println("wysokość lewego poddrzewa: " + (leftHeight() - 1));
            System.out.println("wysokość prawego poddrzewa: " + (rightHeight() - 1));
        }else{
            System.out.println("wysokość drzewa: " + height());
            System.out.println("wysokość lewego poddrzewa: " + leftHeight());
            System.out.println("wysokość prawego poddrzewa: " + rightHeight());
        }
        System.out.println("liczba węzłów: "+nodesCount());
        System.out.println("liczba węzłów lewego poddrzewa: "+leftNodesCount());
        System.out.println("liczba węzłów prawego poddrzewa: "+rightNodesCount());
    }

    public void enterInfoForAllNodes(){
        enterInfoForAllNodes(root);
    }
    private void enterInfoForAllNodes(Node node){
        if (node!=null){
            enterInfoForAllNodes(node.left);
            node.rightNodes=rightNodesCount(node);
            node.leftNodes=leftNodesCount(node);
            node.leftHeight=leftHeight(node);
            node.rightHeight=rightHeight(node);
            enterInfoForAllNodes(node.right);
        }
    }

    public void getInfoForAllNodes(){
        getInfoForAllNodes(root);
    }
    private void getInfoForAllNodes(Node node){
        if (node!=null){
            getInfoForAllNodes(node.left);
            if (node.left==null && node.right==null) {
                System.out.println("Węzeł o kluczu: " + node.key);
                System.out.println("wysokość lewego poddrzewa: " + node.leftHeight);
                System.out.println("wysokość prawego poddrzewa: " + node.rightHeight);
                System.out.println("liczba węzłów lewego poddrzewa: " + node.leftNodes);
                System.out.println("liczba węzłów prawego poddrzewa: " + node.rightNodes);
                System.out.println("------------------------------------------");
            }else{
                System.out.println("Węzeł o kluczu: " + node.key);
                System.out.println("wysokość lewego poddrzewa: " + (node.leftHeight-1));
                System.out.println("wysokość prawego poddrzewa: " + (node.rightHeight-1));
                System.out.println("liczba węzłów lewego poddrzewa: " + node.leftNodes);
                System.out.println("liczba węzłów prawego poddrzewa: " + node.rightNodes);
                System.out.println("------------------------------------------");
            }
            getInfoForAllNodes(node.right);
        }
    }

    public static int getCol(int h) {
        if (h == 1)
            return 1;
        return getCol(h - 1) + getCol(h - 1) + 1;
    }
    private static void displayTree(Node[][] M, Node root, int col, int row, int height) {
        if (root == null)
            return;
        M[row][col] = root;
        displayTree(M, root.left, col - (int)Math.pow(2, height - 2), row + 1, height - 1);
        displayTree(M, root.right, col + (int)Math.pow(2, height - 2), row + 1, height - 1);
    }
    public void displayTree() {
        if (root == null)
            return;
        int h = height(this.root);
        int col = getCol(h);
        Node[][] M = new Node[h][col];
        displayTree(M, this.root, col / 2, 0, h);
        for (int i = 0; i < h; i++) {
            for (int j = 0; j < col; j++) {
                if (M[i][j] == null)
                    System.out.print("  ");
                else
                    if (!M[i][j].isRed){
                        System.out.print(M[i][j].key + " ");
                    }
                    else{
                        System.out.print(ANSI_RED+M[i][j].key + " "+ANSI_RESET);
                    }
            }
            System.out.println();
        }
    }
}