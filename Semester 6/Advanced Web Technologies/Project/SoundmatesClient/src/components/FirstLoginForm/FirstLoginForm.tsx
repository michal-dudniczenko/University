import { useEffect, useState } from "react";
import type { UpdateProfileDto } from "../../types/UpdateProfileDto.ts";
import { useNavigate } from "react-router";
import type { ErrorResponse } from "../../types/ErrorRespose.ts";
import { useUtils } from "../../utils/UtilsContextType.tsx";

function FirstLoginForm() {
    const navigate = useNavigate();

    const { getAccessToken, refreshAccessToken, logout, getUserProfile, updateUserProfileData } = useUtils();

    const [loading, setLoading] = useState(true);
    const [formMessage, setFormMessage] = useState("");
    const [logoutMessage, setLogoutMessage] = useState("");
    const [formData, setFormData] = useState<UpdateProfileDto>({
        id: "",
        name: "",
        description: "",
        birthYear: null,
        city: "",
        country: "",
    });

    const handleLogout = async () => {
        setLogoutMessage("");
        const success = await logout();
        if (!success) {
            setLogoutMessage("Something went wrong... Sign out failed.");
            return;
        }
        navigate("/login");
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        setFormMessage("");

        const url = "http://localhost:5000/users";

        const request = new Request(url, {
            method: "PUT",
            headers: {
                "Access-Token": getAccessToken(),
                "Content-Type": "application/json",
            },
            body: JSON.stringify(formData),
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

        await updateUserProfileData();
        navigate("/");
    };

    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
        const { name, value } = e.target;

        setFormData((prev) => ({
            ...prev,
            [name]: name === "birthYear" ? parseInt(value) : value,
        }));
    };

    useEffect(() => {
        const loadUserProfile = async () => {
            let userProfile = getUserProfile();

            if (!userProfile) {
                await updateUserProfileData();
                userProfile = getUserProfile();
            }

            if (!userProfile) {
                return;
            }

            setFormData((prev) => ({
                ...prev,
                id: userProfile.id,
            }));

            setLoading(false);
        };

        loadUserProfile();
    }, [getUserProfile, updateUserProfileData]);

    if (loading) {
        return (
            <div>
                <h3>Loading...</h3>
                <button onClick={handleLogout}>Sign out</button>
            </div>
        );
    }

    return (
        <div className="m-3 d-flex flex-column">
            <button
                onClick={handleLogout}
                className="btn btn-danger mt-3 mb-3 w-25">
                Sign out
            </button>
            {logoutMessage && <h3>{logoutMessage}</h3>}
            <h1>Set up your profile</h1>
            <form
                onSubmit={handleSubmit}
                className="d-flex flex-column justify-content-center">
                <label htmlFor="name">Name:</label>
                <input
                    type="text"
                    id="name"
                    name="name"
                    value={formData.name}
                    onChange={handleChange}
                    maxLength={50}
                    required
                />

                <label htmlFor="description">Description:</label>
                <textarea
                    id="description"
                    name="description"
                    value={formData.description}
                    onChange={handleChange}
                    maxLength={500}
                    required
                />

                <label htmlFor="birthYear">Birth Year:</label>
                <input
                    type="number"
                    id="birthYear"
                    name="birthYear"
                    value={formData.birthYear ?? ""}
                    onChange={handleChange}
                    min={1900}
                    max={2100}
                    required
                />

                <label htmlFor="city">City:</label>
                <input
                    type="text"
                    id="city"
                    name="city"
                    value={formData.city}
                    onChange={handleChange}
                    maxLength={100}
                    required
                />

                <label htmlFor="country">Country:</label>
                <input
                    type="text"
                    id="country"
                    name="country"
                    value={formData.country}
                    onChange={handleChange}
                    maxLength={100}
                    required
                />
                <button
                    type="submit"
                    className="btn btn-dark mt-3 mb-3">
                    Save
                </button>
                {formMessage && <h3>{formMessage}</h3>}
            </form>
        </div>
    );
}

export default FirstLoginForm;
