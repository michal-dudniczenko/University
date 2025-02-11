public class Request{
    private final int process;
    private final int page;

    public Request(int process, int page){
        this.process = process;
        this.page=page;
    }

    public String toString(){
        return "process: "+ process +" page: "+page;
    }

    public int getProcess() {
        return process;
    }

    public int getPage() {
        return page;
    }
}
