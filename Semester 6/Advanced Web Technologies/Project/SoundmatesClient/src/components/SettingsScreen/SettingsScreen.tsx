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
        <div className="d-flex flex-column p-5">
            <button onClick={handleEditBio} className="btn btn-primary mb-3">Edit Profile</button>
            <button onClick={handleChangePassword} className="btn btn-secondary mb-3">Change Password</button>
            <button onClick={handleDeactivateAccount} className="btn btn-danger mb-3">Deactivate Account</button>
            <button onClick={handleBack} className="btn btn-dark">Back</button>
        </div>
    );
}

export default SettingsScreen;