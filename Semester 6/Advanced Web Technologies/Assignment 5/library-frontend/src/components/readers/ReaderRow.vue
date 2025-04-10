<script>
export default {
    name: "ReaderRow",
    props: {
        reader: Object,
        editReaderId: Number,
        editOnClick: Function,
        deleteOnClick: Function,
        submitEditOnClick: Function,
    },
    data() {
        return {
            editFirstName: this.reader.firstName,
            editLastName: this.reader.lastName,
        }
    }
}
</script>

<template>
    <tr :class="{ 'table-info': reader.id === editReaderId }">
        <td>{{ reader.id }}</td>
        <td>{{ reader.firstName }}</td>
        <td>{{ reader.lastName }}</td>
        <td class="d-flex">
            <button class="btn btn-primary me-4 w-100" @click="editOnClick(reader.id)">Edit</button>
            <button class="btn btn-danger w-100" @click="deleteOnClick(reader.id)">Delete</button>
        </td>
    </tr>
    <tr class="table-info" v-if="editReaderId === reader.id">
        <td></td>
        <td><input class="w-100" type="text" v-model="editFirstName" required></td>
        <td><input class="w-100" type="text" v-model="editLastName" required></td>
        <td>
            <button class="btn btn-success px-5 w-100"
                @click="submitEditOnClick(reader.id, editFirstName, editLastName)">Submit</button>
        </td>
    </tr>
</template>