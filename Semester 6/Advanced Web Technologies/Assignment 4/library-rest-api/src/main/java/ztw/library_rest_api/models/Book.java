package ztw.library_rest_api.models;

public class Book {
    private int id = 0;
    private String title;
    private int authorId;
    private int pages;

    public Book() {}

    public Book(int id, String title, int authorId, int pages) {
        this.id = id;
        this.title = title;
        this.authorId = authorId;
        this.pages = pages;
    }

    public Book(String title, int authorId, int pages) {
        this.title = title;
        this.authorId = authorId;
        this.pages = pages;
    }

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public String getTitle() {
        return title;
    }

    public void setTitle(String title) {
        this.title = title;
    }

    public int getAuthorId() {
        return authorId;
    }

    public void setAuthorId(int authorId) {
        this.authorId = authorId;
    }

    public int getPages() {
        return pages;
    }

    public void setPages(int pages) {
        this.pages = pages;
    }
}
