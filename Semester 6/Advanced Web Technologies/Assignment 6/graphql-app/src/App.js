const { GraphQLServer } = require('graphql-yoga');
const axios = require("axios") 

/*
const usersList = [
    { id: 1, name: "Jan Konieczny", email: "jan.konieczny@wonet.pl", login: "jkonieczny" },
    { id: 2, name: "Anna Wesołowska", email: "anna.w@sad.gov.pl", login: "anna.wesolowska" },
    { id: 3, name: "Piotr Waleczny", email: "piotr.waleczny@gp.pl", login: "p.waleczny" }
];

const todosList = [
    { id: 1, title: "Naprawić samochód", completed: false, user_id: 3 },
    { id: 2, title: "Posprzątać garaż", completed: true, user_id: 3 },
    { id: 3, title: "Napisać e-mail", completed: false, user_id: 3 },
    { id: 4, title: "Odebrać buty", completed: false, user_id: 2 },
    { id: 5, title: "Wysłać paczkę", completed: true, user_id: 2 },
    { id: 6, title: "Zamówic kuriera", completed: false, user_id: 3 },
];
*/

async function todoById(parent, args, context, info) {
    try {
        const response = await fetch(`https://jsonplaceholder.typicode.com/todos/${args.id}`)

        const todo = await response.json()
        return {
            id: todo.id,
            title: todo.title,
            completed: todo.completed,
            user_id: todo.userId,
        }
    } catch (error) {
        throw error
    }
}

async function userById(parent, args, context, info) {
    try {
        const response = await fetch(`https://jsonplaceholder.typicode.com/users/${args.id}`)

        const user = await response.json()
        return {
            id: user.id,
            name: user.name,
            email: user.email,
            login: user.username,
        }
    } catch (error) {
        throw error
    }
}

async function getRestUsersList() {
    try {
        const users = await axios.get('https://jsonplaceholder.typicode.com/users')
        return users.data.map(({ id, name, email, username }) => ({
            id: id,
            name: name,
            email: email,
            login: username,
        }))
    } catch (error) {
        throw error
    }
} 

async function getRestTodosList() {
    try {
        const todos = await axios.get('https://jsonplaceholder.typicode.com/todos')
        return todos.data.map(({ id, title, completed, userId }) => ({
            id: id,
            title: title,
            completed: completed,
            user_id: userId,
        }))
    } catch (error) {
        throw error
    }
} 


const resolvers = {
    Query: {
        users: async () => getRestUsersList(),
        todos: async () => getRestTodosList(),
        todo: async (parent, args, context, info) => todoById(parent, args, context, info),
        user: async (parent, args, context, info) => userById(parent, args, context, info),
    },
    User: {
        todos: async (parent, args, context, info) => {
            return (await getRestTodosList()).filter(t => t.user_id === parent.id);
        }
    },
    ToDoItem: {
        user: async (parent, args, context, info) => {
            return (await getRestUsersList()).find(u => u.id === parent.user_id);
        }
    }
};

const server = new GraphQLServer({
    typeDefs: './src/schema.graphql',
    resolvers,
});

server.start(() => console.log('Server is running on localhost:4000'))