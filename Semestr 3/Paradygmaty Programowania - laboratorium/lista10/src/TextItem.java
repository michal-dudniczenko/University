import java.awt.*;

public class TextItem extends Item{
    private final String text;

    public TextItem(Point position, String text) {
        this.position = position;
        this.text = text;
    }

    public String getText(){
        return this.text;
    }

    @Override
    public void translate(Point p) {
        this.position = new Point(this.position.getX()+p.getX(), this.position.getY()+p.getY());
    }

    @Override
    public Point[] getBoundingBox() {
        int maxRight, maxLeft, maxUp, maxDown;
        maxRight = position.getX()+text.length()*6;
        maxLeft = position.getX()-2;
        maxUp = position.getY()-10;
        maxDown = position.getY()+2;
        return new Point[] {
                new Point(maxLeft, maxUp),
                new Point(maxRight, maxUp),
                new Point(maxRight, maxDown),
                new Point(maxLeft, maxDown)
        };
    }

    @Override
    public void draw(Graphics g) {
        g.setColor(Color.RED);
        g.drawString(text, position.getX(), position.getY());

        if (isBoundVisible){
            drawBoundingBox(g);
        }
    }
}
