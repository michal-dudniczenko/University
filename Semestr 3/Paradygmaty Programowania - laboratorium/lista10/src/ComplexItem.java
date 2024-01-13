import java.awt.*;
import java.util.ArrayList;

public class ComplexItem extends Item{
    private final ArrayList<Item> children;

    public ComplexItem(ArrayList<Item> children) {
        int maxLeft, maxUp;
        if (!children.isEmpty()){
            maxLeft = children.getFirst().getPosition().getX();
            maxUp = children.getFirst().getPosition().getY();
        }else{
            maxLeft = 0;
            maxUp = 0;
        }
        for (int i=1; i<children.size(); i++){
            if (children.get(i).getPosition().getX() < maxLeft){
                maxLeft = children.get(i).getPosition().getX();
            }
            if (children.get(i).getPosition().getY() < maxUp){
                maxUp = children.get(i).getPosition().getY();
            }
        }
        this.position = new Point(maxLeft, maxUp);
        this.children = children;
    }

    public ArrayList<Item> getChildren(){
        return this.children;
    }

    @Override
    public void translate(Point p) {
        this.position = new Point(this.position.getX()+p.getX(), this.position.getY()+p.getY());
        for (Item item : children){
            item.translate(p);
        }
    }

    @Override
    public Point[] getBoundingBox() {
        int maxRight, maxLeft, maxUp, maxDown;
        maxLeft = this.position.getX();
        maxUp = this.position.getY();
        if (!children.isEmpty()){
            maxRight = children.getFirst().getBoundingBox()[1].getX();
            maxDown = children.getFirst().getBoundingBox()[3].getY();
        }else{
            maxRight = 0;
            maxDown = 0;
        }
        for (int i=1; i<children.size(); i++){
            if (children.get(i).getBoundingBox()[1].getX() > maxRight){
                maxRight = children.get(i).getBoundingBox()[1].getX();
            }
            if (children.get(i).getBoundingBox()[2].getY() > maxDown){
                maxDown = children.get(i).getBoundingBox()[2].getY();
            }
        }
        return new Point[] {
                new Point(maxLeft, maxUp),
                new Point(maxRight, maxUp),
                new Point(maxRight, maxDown),
                new Point(maxLeft, maxDown)
        };
    }

    @Override
    public void draw(Graphics g) {
        for (Item item : children){
            item.draw(g);
        }

        if (isBoundVisible){
            drawBoundingBox(g);
        }
    }
}