import { useNavigate } from "react-router";

function SettingsScreen() {
    const navigate = useNavigate();

    const handleEditBio = () => {
        navigate("/edit-profile");
    }

    const handleChangePassword = () => {
        navigate("/change-password");
    };

    const handleDeactivateAccount = () => {
        navigate("/deactivate");
    };

    const handleBack = () => {
        navigate("/");
    };

    return (
        <div>
            <button onClick={handleEditBio}>Edit Profile</button>
            <button onClick={handleChangePassword}>Change Password</button>
            <button onClick={handleDeactivateAccount}>Deactivate Account</button>
            <button onClick={handleBack}>Back</button>
        </div>
    );
}

export default SettingsScreen;