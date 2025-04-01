import java.awt.*;

public abstract class Item {
    protected Point position;
    protected boolean isBoundVisible=false;

    public Point getPosition(){
        return this.position;
    }

    public void setBoundVisible(boolean boundVisibility){
        this.isBoundVisible = boundVisibility;
    }

    public abstract void translate(Point p);
    public abstract Point[] getBoundingBox();
    public abstract void draw(Graphics g);

    protected void drawBoundingBox(Graphics g){
        Graphics2D g2d = (Graphics2D) g;
        g2d.setColor(Color.BLUE);

        Point[] boundingBox = getBoundingBox();

        int[] xPoints = new int[4];
        int[] yPoints = new int[4];

        for (int i=0; i<boundingBox.length; i++){
            xPoints[i] = boundingBox[i].getX();
            yPoints[i] = boundingBox[i].getY();
        }
        Stroke originalStroke = g2d.getStroke();
        Stroke dashedStroke = new BasicStroke(1, BasicStroke.CAP_BUTT, BasicStroke.JOIN_BEVEL, 0, new float[]{5}, 0);
        g2d.setStroke(dashedStroke);

        g2d.drawPolygon(xPoints, yPoints, 4);

        g2d.setStroke(originalStroke);
    }
}