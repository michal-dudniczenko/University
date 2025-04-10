<script>
export default {
    name: "AuthorRow",
    props: {
        author: Object,
        editAuthorId: Number,
        editOnClick: Function,
        deleteOnClick: Function,
        submitEditOnClick: Function,
    },
    data() {
        return {
            editFirstName: this.author.firstName,
            editLastName: this.author.lastName,
        }
    }
}
</script>

<template>
    <tr :class="{ 'table-info': author.id === editAuthorId }">
        <td>{{ author.id }}</td>
        <td>{{ author.firstName }}</td>
        <td>{{ author.lastName }}</td>
        <td class="d-flex">
            <button class="btn btn-primary me-4 w-100" @click="editOnClick(author.id)">Edit</button>
            <button class="btn btn-danger w-100" @click="deleteOnClick(author.id)">Delete</button>
        </td>
    </tr>
    <tr class="table-info" v-if="editAuthorId === author.id">
        <td></td>
        <td><input class="w-100" type="text" v-model="editFirstName" required></td>
        <td><input class="w-100" type="text" v-model="editLastName" required></td>
        <td>
            <button class="btn btn-success px-5 w-100"
                @click="submitEditOnClick(author.id, editFirstName, editLastName)">Submit</button>
        </td>
    </tr>
</template>