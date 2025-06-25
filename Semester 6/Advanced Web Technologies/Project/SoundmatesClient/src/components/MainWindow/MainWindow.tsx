import { useEffect, useState } from "react";
import { useNavigate } from "react-router";
import { useUtils } from "../../utils/UtilsContextType";

function MainWindow() {
    const navigate = useNavigate();
    const { logout, getUserProfile, updateUserProfileData } = useUtils();

    const [loading, setLoading] = useState(true);
    const [logoutMessage, setLogoutMessage] = useState("");

    const handleLogout = async () => {
        setLogoutMessage("");
        const success = logout();
        if (!success) {
            setLogoutMessage("Something went wrong... Sign out failed.");
            return;
        }
        navigate("/login");
    };

    const openSettings = () => {
        navigate("/settings");
    }

    useEffect(() => {
        const checkAndNavigate = async () => {
            let userProfile = getUserProfile();

            if (!userProfile) {
                await updateUserProfileData();
                userProfile = getUserProfile();
            }

            if (!userProfile) {
                return;
            }

            if (userProfile.isFirstLogin) {
                navigate("/first-login", {
                    state: { userId: userProfile.id },
                });
                return;
            }

            setLoading(false);
        };

        checkAndNavigate();
    }, [getUserProfile, updateUserProfileData, navigate, logout]);

    if (loading) {
        return <div>Loading...</div>;
    }

    return (
        <div>
            <button onClick={openSettings}>Settings</button>
            <button onClick={handleLogout}>Sign out</button>
            {logoutMessage && <h3>{logoutMessage}</h3>}
            <h1>hello, main app Soundmates</h1>
        </div>
    );
}

export default MainWindow;
