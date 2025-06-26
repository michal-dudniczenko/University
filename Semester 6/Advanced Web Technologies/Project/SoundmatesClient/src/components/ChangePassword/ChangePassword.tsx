import React, { useState } from "react";
import { useNavigate } from "react-router";
import type { ErrorResponse } from "../../types/ErrorRespose";
import { useUtils } from "../../utils/UtilsContextType";
import type { ChangePasswordDto } from "../../types/ChangePasswordDto";

function RegisterForm() {
    const navigate = useNavigate();

    const { getAccessToken, refreshAccessToken } = useUtils();

    const [oldPassword, setOldPassword] = useState("");
    const [newPasword, setNewPassword] = useState("");
    const [confirmNewPasword, setConfirmNewPassword] = useState("");
    const [formMessage, setFormMessage] = useState("");

    const handleBack = () => {
        navigate("/settings");
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        setFormMessage("");

        if (newPasword !== confirmNewPasword) {
            setFormMessage("Confirm password is different than password.");
            return;
        }

        const url = "http://localhost:5000/users/change-password";

        const dto: ChangePasswordDto = {
            oldPassword: oldPassword,
            newPassword: newPasword,
        };

        const request = new Request(url, {
            method: "POST",
            headers: {
                "Access-Token": getAccessToken(),
                "Content-Type": "application/json",
            },
            body: JSON.stringify(dto),
        });

        let response = await fetch(request);

        if (response.status === 401) {
            const refreshed = refreshAccessToken();
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

        setFormMessage("Password changed.");
    };

    return (
        <div>
            <button onClick={handleBack}>Back</button>
            <h3>Change your password</h3>
            <form onSubmit={handleSubmit}>
                <div>
                    <label>Old password:</label>
                    <input
                        type="password"
                        value={oldPassword}
                        onChange={(e) => setOldPassword(e.target.value)}
                        required
                    />
                </div>
                <div>
                    <label>New password:</label>
                    <input
                        type="password"
                        value={newPasword}
                        onChange={(e) => setNewPassword(e.target.value)}
                        required
                    />
                </div>
                <div>
                    <label>Confirm new password:</label>
                    <input
                        type="password"
                        value={confirmNewPasword}
                        onChange={(e) => setConfirmNewPassword(e.target.value)}
                        required
                    />
                </div>
                <button type="submit">Change password</button>
                {formMessage && <h3>{formMessage}</h3>}
            </form>
        </div>
    );
}

export default RegisterForm;
