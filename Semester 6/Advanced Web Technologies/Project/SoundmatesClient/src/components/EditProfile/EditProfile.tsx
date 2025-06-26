import { useEffect, useState } from "react";
import type { UpdateProfileDto } from "../../types/UpdateProfileDto.ts";
import { useNavigate } from "react-router";
import type { ErrorResponse } from "../../types/ErrorRespose.ts";
import { useUtils } from "../../utils/UtilsContextType.tsx";

function EditProfile() {
    const navigate = useNavigate();

    const { getAccessToken, refreshAccessToken, getUserProfile, updateUserProfileData } = useUtils();
    const [loading, setLoading] = useState(true);

    const [formMessage, setFormMessage] = useState("");
    const [formData, setFormData] = useState<UpdateProfileDto>({
        id: "",
        name: "",
        description: "",
        birthYear: null,
        city: "",
        country: "",
    });

    const handleBack = () => {
        navigate("/settings");
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

        updateUserProfileData();
        setFormMessage("Profile updated successfully.");
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

            setFormData({
                id: userProfile.id,
                name: userProfile.name,
                description: userProfile.description,
                birthYear: userProfile.birthYear,
                city: userProfile.city,
                country: userProfile.country,
            });

            setLoading(false);
        };

        loadUserProfile();
    }, [getUserProfile, updateUserProfileData]);

    if (loading) {
        return (
            <div>
                <h3>Loading...</h3>
                <button onClick={handleBack}>Back</button>
            </div>
        );
    }

    return (
        <div>
            <button onClick={handleBack}>Back</button>
            <h1>Edit your profile information</h1>
            <form onSubmit={handleSubmit}>
                <label htmlFor="name">Name:</label>
                <br />
                <input
                    type="text"
                    id="name"
                    name="name"
                    value={formData.name}
                    onChange={handleChange}
                    maxLength={50}
                    required
                />
                <br />
                <br />

                <label htmlFor="description">Description:</label>
                <br />
                <textarea
                    id="description"
                    name="description"
                    value={formData.description}
                    onChange={handleChange}
                    maxLength={500}
                    required
                />
                <br />
                <br />

                <label htmlFor="birthYear">Birth Year:</label>
                <br />
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
                <br />
                <br />

                <label htmlFor="city">City:</label>
                <br />
                <input
                    type="text"
                    id="city"
                    name="city"
                    value={formData.city}
                    onChange={handleChange}
                    maxLength={100}
                    required
                />
                <br />
                <br />

                <label htmlFor="country">Country:</label>
                <br />
                <input
                    type="text"
                    id="country"
                    name="country"
                    value={formData.country}
                    onChange={handleChange}
                    maxLength={100}
                    required
                />
                <br />
                <br />
                <button type="submit">Submit</button>
                {formMessage && <h3>{formMessage}</h3>}
            </form>
        </div>
    );
}

export default EditProfile;
