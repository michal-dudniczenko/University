import React, { useState } from "react";
import { useNavigate } from "react-router";
import type { ErrorResponse } from "../../types/ErrorRespose";
import type { LoginRegisterDto } from "../../types/LoginRegisterDto";
import { useUtils } from "../../utils/UtilsContextType";
import type { AccessTokenDto } from "../../types/AccessTokenDto";

function RegisterForm() {
    const navigate = useNavigate();
    const { updateAccessToken } = useUtils();

    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");
    const [message, setMessage] = useState("");

    const navigateToLogin = () => {
        navigate("/login");
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        setMessage("");

        if (password !== confirmPassword) {
            setMessage("Confirm password is different than password.");
            return;
        }

        const url = "http://localhost:5000/users/register";

        const dto: LoginRegisterDto = {
            email: email,
            password: password,
        };

        const response = await fetch(url, {
            method: "POST",
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
        <div className="m-3 d-flex flex-column">
            <h1>Register new account</h1>
            <form
                onSubmit={handleSubmit}
                className="d-flex flex-column justify-content-center">
                <label>Email address:</label>
                <input
                    type="email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    required
                />
                <label>Password:</label>
                <input
                    type="password"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    autoComplete="new-password"
                    required
                />
                <label>Confirm password:</label>
                <input
                    type="password"
                    value={confirmPassword}
                    onChange={(e) => setConfirmPassword(e.target.value)}
                    autoComplete="new-password"
                    required
                />
                <button
                    type="submit"
                    className="btn btn-primary mt-3 mb-3">
                    Register
                </button>
                {message && <h3>{message}</h3>}
            </form>
            Already have an account?{" "}
            <button
                onClick={navigateToLogin}
                className="btn btn-dark">
                Log in
            </button>
        </div>
    );
}

export default RegisterForm;
