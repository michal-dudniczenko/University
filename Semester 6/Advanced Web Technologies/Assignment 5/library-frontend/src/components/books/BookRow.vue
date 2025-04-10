<script>
    export default {
        name: "BookRow",
        props: {
            book: Object,
            editBookId: Number,
            editOnClick: Function,
            deleteOnClick: Function,
            submitEditOnClick: Function,
        },
        data() {
            return {
                editTitle: this.book.title,
                editAuthorId: this.book.authorId,
                editPages: this.book.pages,
            }
        }
    }
</script>

<template>
    <tr :class="{ 'table-info' : book.id === editBookId }" >
        <td>{{ book.id }}</td>
        <td>{{ book.title }}</td>
        <td>{{ book.authorId }}</td>
        <td>{{ book.pages }}</td>
        <td class="d-flex">
            <button class="btn btn-primary me-4 w-100" @click="editOnClick(book.id)">Edit</button>
            <button class="btn btn-danger w-100" @click="deleteOnClick(book.id)">Delete</button>
        </td>
    </tr>
    <tr class="table-info" v-if="editBookId === book.id">
        <td></td>
        <td><input class="w-100" type="text" v-model="editTitle" required></td>
        <td><input class="w-100" type="number" v-model="editAuthorId" required></td>
        <td><input class="w-100" type="number" v-model="editPages" required></td>
        <td>
            <button class="btn btn-success px-5 w-100" @click="submitEditOnClick(book.id, editTitle, editAuthorId, editPages)" >Submit</button>
        </td>
    </tr>
</template>