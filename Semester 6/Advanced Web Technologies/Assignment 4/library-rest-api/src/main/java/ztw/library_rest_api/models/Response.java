package ztw.library_rest_api.models;

import org.springframework.http.HttpStatus;

public class Response {
    private HttpStatus status;
    private Object payload = null;

    public Response(HttpStatus status, Object payload) {
        this.status = status;
        this.payload = payload;
    }

    public Response(HttpStatus status) {
        this.status = status;
    }

    public HttpStatus getStatus() {
        return status;
    }

    public void setStatus(HttpStatus status) {
        this.status = status;
    }

    public Object getPayload() {
        return payload;
    }

    public void setPayload(Object payload) {
        this.payload = payload;
    }
}
