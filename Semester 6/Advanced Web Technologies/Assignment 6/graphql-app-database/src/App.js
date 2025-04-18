const { GraphQLServer } = require('graphql-yoga');
const pool = require('./db.js')


async function initData() {
    try {
        // 1. Fetch data
        const usersResponse = await fetch('https://jsonplaceholder.typicode.com/users');
        const todosResponse = await fetch('https://jsonplaceholder.typicode.com/todos');

        const users = await usersResponse.json();
        const todos = await todosResponse.json();

        // 2. Insert users
        for (const user of users) {
        await pool.query(
            `INSERT INTO users (id, name, email, login)
            VALUES ($1, $2, $3, $4)
            ON CONFLICT (id) DO NOTHING`,
            [user.id, user.name, user.email, user.username]
        );
        }

        // 3. Insert todos
        for (const todo of todos) {
        await pool.query(
            `INSERT INTO todos (id, title, completed, user_id)
            VALUES ($1, $2, $3, $4)
            ON CONFLICT (id) DO NOTHING`,
            [todo.id, todo.title, todo.completed, todo.userId]
        );
        }

        console.log('✅ Data imported successfully!');
    } catch (error) {
        console.error('❌ Error importing data:', error.message);
    }
}


const resolvers = {
    Query: {
        users: async () => {
            return (await pool.query('SELECT * FROM users')).rows;
        },
        todos: async () => {
            return (await pool.query('SELECT * FROM todos')).rows;
        },
        user: async (parent, args, context, info) => {
            return (await pool.query('SELECT * FROM users WHERE id = $1', [args.id])).rows[0];
        },
        todo: async (parent, args, context, info) => {
            return (await pool.query('SELECT * FROM todos WHERE id = $1', [args.id])).rows[0];
        },
    },
    Mutation: {
        createUser: async (parent, args, context, info) => {
            try {
                const currentMaxId = (await pool.query('SELECT MAX(id) FROM users')).rows[0].max || 1
                await pool.query(
                    `INSERT INTO users (id, name, email, login)
                     VALUES ($1, $2, $3, $4)
                    `, [currentMaxId + 1, args.name, args.email, args.login]
                )
                return true;
            } catch (error) {
                console.error("error:", error)
                return false;
            }
        },
        updateUser: async (parent, args, context, info) => {
            try {
                const user = await pool.query('SELECT * FROM users WHERE id = $1', [args.id]);
                if (user.rows.length === 0) throw new Error('User not found');

                await pool.query(
                    `
                        UPDATE users 
                        SET
                            name = $2,
                            email = $3,
                            login = $4
                        WHERE id = $1
                    `, [args.id, args.name, args.email, args.login]
                )
                return true;
            } catch (error) {
                console.log(error)
                return false;
            }
        },
        deleteUser: async (parent, args, context, info) => {
            try {
                const user = await pool.query('SELECT * FROM users WHERE id = $1', [args.id]);
                if (user.rows.length === 0) throw new Error('User not found');

                await pool.query(`DELETE FROM users WHERE id = $1`, [args.id]);
                return true;
            } catch (error) {
                console.log(error);
                return false;
            }
        },
        createTodo: async (parent, args, context, info) => {
            try {
                const currentMaxId = (await pool.query('SELECT MAX(id) FROM todos')).rows[0].max || 1
                await pool.query(
                    `INSERT INTO todos (id, title, completed, user_id)
                     VALUES ($1, $2, $3, $4)
                    `, [currentMaxId + 1, args.title, args.completed, args.userId]
                )
                return true;
            } catch (error) {
                console.error("error:", error)
                return false;
            }
        },
        updateTodo: async (parent, args, context, info) => {
            try {
                const todo = await pool.query('SELECT * FROM todos WHERE id = $1', [args.id]);
                if (todo.rows.length === 0) throw new Error('Todo not found');

                await pool.query(
                    `
                        UPDATE todos 
                        SET
                            title = $2,
                            completed = $3,
                            user_id = $4
                        WHERE id = $1
                    `, [args.id, args.title, args.completed, args.userId]
                )
                return true;
            } catch (error) {
                console.log(error)
                return false;
            }
        },
        deleteTodo: async (parent, args, context, info) => {
            try {
                const todo = await pool.query('SELECT * FROM todos WHERE id = $1', [args.id]);
                if (todo.rows.length === 0) throw new Error('Todo not found');

                await pool.query(`DELETE FROM todos WHERE id = $1`, [args.id]);
                return true;
            } catch (error) {
                console.log(error);
                return false;
            }
        },
    },
    User: {
        todos: async (parent, args, context, info) => {
            return (await pool.query('SELECT * FROM todos WHERE user_id = $1', [parent.id])).rows;
        }
    },
    ToDoItem: {
        user: async (parent, args, context, info) => {
            return (await pool.query('SELECT * FROM users WHERE id = $1', [parent.user_id])).rows[0];
        }
    },
};


const server = new GraphQLServer({
    typeDefs: './src/schema.graphql',
    resolvers,
});

server.start(() => console.log('Server is running on localhost:4000'))