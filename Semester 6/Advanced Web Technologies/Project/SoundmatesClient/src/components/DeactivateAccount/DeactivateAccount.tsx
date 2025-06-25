import React, { useState } from "react";
import { useNavigate } from "react-router";
import type { ErrorResponse } from "../../types/ErrorRespose";
import { useUtils } from "../../utils/UtilsContextType";
import type { LoginRegisterDto } from "../../types/LoginRegisterDto";

function DeactivateAccount() {
    const navigate = useNavigate();

    const { getAccessToken, refreshAccessToken, logout } = useUtils();

    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [formMessage, setFormMessage] = useState("");

    const handleBack = () => {
        navigate("/settings");
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        setFormMessage("");

        const url = "http://localhost:5000/users";

        const dto: LoginRegisterDto = {
            email: email,
            password: password,
        };

        const request = new Request(url, {
            method: "DELETE",
            credentials: "include",
            headers: {
                "Access-Token": getAccessToken(),
                "Content-Type": "application/json",
            },
            body: JSON.stringify(dto),
        });

        let response = await fetch(request);

        if (response.status === 401) {
            const refreshed = await refreshAccessToken();
            if (!refreshed) {
                setFormMessage("Something went wrong...");
                return;
            }
            response = await fetch(request);
        }

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

            setFormMessage(errorMessage);
            return;
        }

        await logout();
        navigate("/login");
    };

    return (
        <div>
            <button onClick={handleBack}>Back</button>
            <h3>Deactivate your account</h3>
            <form
                onSubmit={handleSubmit}
                autoComplete="off">
                <div>
                    <label>Email address:</label>
                    <input
                        type="email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        required
                        autoComplete="off"
                    />
                </div>
                <div>
                    <label>Password:</label>
                    <input
                        type="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        required
                        autoComplete="new-password"
                    />
                </div>
                <button type="submit">Deactivate</button>
                {formMessage && <h3>{formMessage}</h3>}
            </form>
        </div>
    );
}

export default DeactivateAccount;
