import { useState } from "react";
import { useNavigate } from "react-router";
import type { ErrorResponse } from "../../types/ErrorRespose";
import type { LoginRegisterDto } from "../../types/LoginRegisterDto";
import type { AccessTokenDto } from "../../types/AccessTokenDto";
import { useUtils } from "../../utils/UtilsContextType";

function LoginForm() {
    const navigate = useNavigate();
    const { updateAccessToken } = useUtils();

    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [message, setMessage] = useState("");

    const navigateToRegistration = () => {
        navigate("/register");
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        setMessage("");

        const url = "http://localhost:5000/users/login";

        const dto: LoginRegisterDto = {
            email: email,
            password: password
        }

        const response = await fetch(url, {
            method: "POST",
            credentials: "include",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(dto),
        });

        if (!response.ok) {
            let errorMessage = "Something went wrong...";

            const data = (await response.json()) as ErrorResponse;

            if (data.message) {
                errorMessage = data.message;
            } else if (data.errors && typeof data.errors === "object") {
                // Flatten all error messages and get the first one
                const allErrors = Object.values(data.errors).flat();
                if (allErrors.length > 0) {
                    errorMessage = allErrors[0];
                }
            }

            setMessage(errorMessage);
            return;
        }

        const data = (await response.json()) as AccessTokenDto;
        updateAccessToken(data.accessToken);

        navigate("/");
    };

    return (
        <div>
            <h3>Log in to your account</h3>
            <form onSubmit={handleSubmit}>
                <div>
                    <label>Email address:</label>
                    <input
                        type="email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        required
                    />
                </div>
                <div>
                    <label>Password:</label>
                    <input
                        type="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        required
                    />
                </div>
                <button type="submit">Log in</button>
                {message && <h3>{message}</h3>}
            </form>
            <button onClick={navigateToRegistration}>Create a new account</button>
        </div>
    );
}

export default LoginForm;
