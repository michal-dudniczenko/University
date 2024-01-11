import java.util.List;

public class ComplexItem extends Item{
    private List<Item> children;

    public ComplexItem(List<Item> children) {
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
            if (children.get(i).getPosition().getY() > maxUp){
                maxUp = children.get(i).getPosition().getY();
            }
        }
        this.position = new Point(maxLeft, maxUp);
        this.children = children;
    }

    public List<Item> getChildren(){
        return this.children;
    }

    @Override
    public Point getPosition() {
        return this.position;
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
            maxRight = children.getFirst().getPosition().getX();
            maxDown = children.getFirst().getPosition().getY();
        }else{
            maxRight = 0;
            maxDown = 0;
        }
        for (int i=1; i<children.size(); i++){
            if (children.get(i).getPosition().getX() > maxRight){
                maxRight = children.get(i).getPosition().getX();
            }
            if (children.get(i).getPosition().getY() < maxDown){
                maxDown = children.get(i).getPosition().getY();
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
    public void draw() {

    }
}
