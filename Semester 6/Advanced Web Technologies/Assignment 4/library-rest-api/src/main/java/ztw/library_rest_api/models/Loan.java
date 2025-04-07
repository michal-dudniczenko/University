package ztw.library_rest_api.models;

public class Loan {
    private int id = 0;
    private int readerId;
    private int bookId;
    private boolean isReturned = false;

    public Loan() {}

    public Loan(int id, int readerId, int bookId) {
        this.id = id;
        this.readerId = readerId;
        this.bookId = bookId;
    }

    public Loan(int readerId, int bookId) {
        this.readerId = readerId;
        this.bookId = bookId;
    }

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public int getReaderId() {
        return readerId;
    }

    public void setReaderId(int readerId) {
        this.readerId = readerId;
    }

    public int getBookId() {
        return bookId;
    }

    public void setBookId(int bookId) {
        this.bookId = bookId;
    }

    public boolean isReturned() {
        return isReturned;
    }

    public void setReturned(boolean isReturned) {
        this.isReturned = isReturned;
    }
}
