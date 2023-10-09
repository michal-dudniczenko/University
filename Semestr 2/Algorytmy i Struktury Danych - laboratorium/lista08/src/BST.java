import java.util.NoSuchElementException;

public class BST {
    private class KeyAlreadyInTreeException extends RuntimeException{}

    public class Node{
        private int key;
        private Node left;
        private Node right;

        public Node(int key){
            this.key=key;
            this.left=this.right=null;
        }

        public int getKey(){
            return this.key;
        }

        public void setKey(int key){
            this.key=key;
        }

        public Node getLeft() {
            return left;
        }

        public void setLeft(Node left) {
            this.left = left;
        }

        public Node getRight() {
            return right;
        }

        public void setRight(Node right) {
            this.right = right;
        }

        public void increaseKey(){
            this.key++;
        }
    }

    private Node root;

    public BST(){
        this.root=null;
    }

    public void insert(int key){
        Node node = new Node(key);
        if (this.root==null)this.root=node;
        else{
            Node prev = null;
            Node temp = this.root;
            while(temp!=null){
                prev=temp;
                if (key==temp.key)throw new KeyAlreadyInTreeException();
                else if (key<temp.key)temp=temp.left;
                else temp=temp.right;
            }
            if (key<prev.key)prev.left=node;
            else prev.right=node;
        }
    }

    public void displayInOrder(){
        System.out.print("\n");
        displayInOrder(this.root);
        System.out.print("\n");
    }
    private void displayInOrder(Node node){
        if (node!=null){
            displayInOrder(node.left);
            System.out.print(node.key+" ");
            displayInOrder(node.right);
        }
    }

    public void displayPreOrder(){
        System.out.print("\n");
        displayPreOrder(this.root);
        System.out.print("\n");
    }
    private void displayPreOrder(Node node){
        if (node!=null){
            System.out.print(node.key+" ");
            displayPreOrder(node.left);
            displayPreOrder(node.right);
        }
    }

    public void displayPostOrder(){
        System.out.print("\n");
        displayPostOrder(this.root);
        System.out.print("\n");
    }
    private void displayPostOrder(Node node){
        if (node!=null){
            displayPostOrder(node.left);
            displayPostOrder(node.right);
            System.out.print(node.key+" ");
        }
    }

    public boolean search(int key){
        Node temp = this.root;

        while(temp!=null && temp.key!=key){
            if (key<temp.key)temp=temp.left;
            else temp=temp.right;
        }
        return temp!=null;
    }

    public int minValue(){
        if (this.root==null)throw new NoSuchElementException();
        Node temp = this.root;
        Node prev = null;
        while(temp!=null){
            prev=temp;
            temp=temp.left;
        }
        return prev.key;
    }

    public int maxValue(){
        if (this.root==null)throw new NoSuchElementException();
        Node temp = this.root;
        Node prev = null;
        while(temp!=null){
            prev=temp;
            temp=temp.right;
        }
        return prev.key;
    }

    public void displayInfo(){
        System.out.println("Wysokość drzewa = "+height(root));
        System.out.println("Liczba węzłów = "+nodesCount(root));
        System.out.println("Liczba liści = "+leavesCount(root));
    }

    private static int height(Node node){
        if (node==null){
            return 0;
        }else{
            int leftHeight=height(node.left);
            int rightHeight=height(node.right);

            if (leftHeight>rightHeight){
                return leftHeight+1;
            }else{
                return rightHeight+1;
            }
        }
    }

    private static int nodesCount(Node node){
        if (node==null){
            return 0;
        }else{
            return 1+nodesCount(node.left)+nodesCount(node.right);
        }
    }

    private static int leavesCount(Node node){
        if (node==null){
            return 0;
        }else{
            if (node.left==null && node.right==null){
                return 1;
            }else{
                return leavesCount(node.left)+leavesCount(node.right);
            }
        }
    }

    public static int getCol(int h) {
        if (h == 1)
            return 1;
        return getCol(h - 1) + getCol(h - 1) + 1;
    }

    private static void displayTree(Integer[][] M, Node root, int col, int row, int height) {
        if (root == null)
            return;
        M[row][col] = root.key;
        displayTree(M, root.left, col - (int)Math.pow(2, height - 2), row + 1, height - 1);
        displayTree(M, root.right, col + (int)Math.pow(2, height - 2), row + 1, height - 1);
    }

    public void displayTree() {
        int h = height(this.root);
        int col = getCol(h);
        Integer[][] M = new Integer[h][col];
        displayTree(M, this.root, col / 2, 0, h);
        for (int i = 0; i < h; i++) {
            for (int j = 0; j < col; j++) {
                if (M[i][j] == null)
                    System.out.print("  ");
                else
                    System.out.print(M[i][j] + " ");
            }
            System.out.println();
        }
    }

    public void heightAndNodesForAll(){
        heightAndNodesForAll(this.root);
    }

    private void heightAndNodesForAll(Node node){
        if (node!=null){
            heightAndNodesForAll(node.left);
            System.out.println("Węzeł: "+node.key+" wysokość poddrzewa: "+height(node)+" liczba węzłów poddrzewa: "+nodesCount(node));
            heightAndNodesForAll(node.right);
        }
    }

    private static void walkInOrder(Node node, Node[] nodes) {
        if (node != null) {
            walkInOrder(node.left, nodes);
            nodes[nodes[nodes.length - 1].getKey()] = node;
            nodes[nodes.length - 1].increaseKey();
            walkInOrder(node.right, nodes);
        }
    }

    public Node nastepnik(int key) {
        Node[] temp = new Node[nodesCount(this.root) + 1];
        temp[temp.length-1]=new Node(0);
        walkInOrder(this.root, temp);
        for (int i = 0; i < temp.length - 1; i++) {
            if (temp[i].getKey() == key) {
                if (i == temp.length - 2) {
                    return new Node(-1);
                }else {
                    return temp[i + 1];
                }
            }
        }
        return new Node(-2);
    }

    public Node poprzednik(int key){
        Node[] temp = new Node[nodesCount(this.root) + 1];
        temp[temp.length-1]=new Node(0);
        walkInOrder(this.root, temp);
        for (int i = 0; i < temp.length - 1; i++) {
            if (temp[i].getKey() == key) {
                if (i == 0) {
                    return new Node(-1);
                }else {
                    return temp[i - 1];
                }
            }
        }
        return new Node(-2);
    }

    public void remove(int key){
        Node temp = this.root;
        Node prev = null;

        while(temp.key!=key){
            if (key<temp.key){
                prev=temp;
                temp=temp.left;
            }
            else{
                prev=temp;
                temp=temp.right;
            }
        }
        if (temp.getLeft()==null && temp.getRight()==null){
            if (prev!=null && prev.getLeft() != null && prev.getLeft().getKey()==temp.getKey()){
                prev.setLeft(null);
            }else if (prev!=null){
                prev.setRight(null);
            }
        }else if (temp.getLeft()==null){
            if (prev!=null && prev.getLeft()!=null && prev.getLeft().getKey()==temp.getKey()){
                prev.setLeft(temp.getRight());
            }else if (prev!=null){
                prev.setRight(temp.getRight());
            }
        }else if (temp.getRight()==null) {
            if (prev!=null && prev.getLeft().getKey() == temp.getKey()) {
                prev.setLeft(temp.getLeft());
            } else if (prev!=null){
                prev.setRight(temp.getLeft());
            }
        }else{
            Node nast = nastepnik(key);
            Node nast2 = nastepnik(nast.getKey());
            if (prev!=null) {
                if (prev.getKey() > nast.getKey()) {
                    prev.setLeft(nast);
                } else {
                    prev.setRight(nast);
                }
                nast.setLeft(temp.getLeft());
            }else{
                nast.setLeft(temp.getLeft());
                nast.setRight(temp.getRight());
                this.root=nast;
                nast2.setLeft(null);
            }
        }
    }
}
