public class BezierCurve extends Shape{
    private Point[] controlPoints;

    public BezierCurve(boolean isFilled, Point[] controlPoints) {
        int maxLeft, maxUp;
        if (controlPoints.length > 0){
            maxLeft = controlPoints[0].getX();
            maxUp = controlPoints[0].getY();
        }else{
            maxLeft = 0;
            maxUp = 0;
        }
        for (int i=1; i<controlPoints.length; i++){
            if (controlPoints[i].getX() < maxLeft){
                maxLeft = controlPoints[i].getX();
            }
            if (controlPoints[i].getY() > maxUp){
                maxUp = controlPoints[i].getY();
            }
        }

        this.position = new Point(maxLeft, maxUp);
        this.isFilled = isFilled;
        this.controlPoints = controlPoints;
    }

    public Point[] getControlPoints(){
        return this.controlPoints;
    }

    @Override
    public boolean getFilled() {
        return this.isFilled;
    }

    @Override
    public Point getPosition() {
        return this.position;
    }


    @Override
    public void translate(Point p) {
        this.position = new Point(this.position.getX()+p.getX(), this.position.getY()+p.getY());
        for (int i=0; i<controlPoints.length; i++){
            controlPoints[i] = new Point(controlPoints[i].getX()+p.getX(), controlPoints[i].getY()+p.getY());
        }
    }

    @Override
    public Point[] getBoundingBox() {
        int maxRight, maxLeft, maxUp, maxDown;
        maxLeft = this.position.getX();
        maxUp = this.position.getY();
        if (controlPoints.length > 0){
            maxRight = controlPoints[0].getX();
            maxDown = controlPoints[0].getY();
        }else{
            maxRight = 0;
            maxDown = 0;
        }
        for (int i=1; i<controlPoints.length; i++){
            if (controlPoints[i].getX() > maxRight){
                maxRight = controlPoints[i].getX();
            }
            if (controlPoints[i].getY() < maxDown){
                maxDown = controlPoints[i].getY();
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
