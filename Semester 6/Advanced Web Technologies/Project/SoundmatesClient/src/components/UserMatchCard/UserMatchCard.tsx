import type { OtherUserProfileDto } from "../../types/OtherUserProfileDto";

interface Props {
    userProfile: OtherUserProfileDto;
    handleLike: () => Promise<void>;
    handleDislike: () => Promise<void>;
}

function UserMatchCard({ userProfile, handleLike, handleDislike }: Props) {
    return (
        <div className="m-5 border bg-dark p-3 rounded">
            <h2 className="text-white">
                {userProfile.name}, {new Date().getFullYear() - userProfile.birthYear} lat
            </h2>
            <h5 className="text-white">
                {userProfile.city}, {userProfile.country}
            </h5>
            <h5 className="text-white">Opis:</h5>
            <p className="text-white">{userProfile.description}</p>
            <div>
                <button
                    onClick={handleLike}
                    className="btn btn-success me-3">
                    Like
                </button>
                <button
                    onClick={handleDislike}
                    className="btn btn-danger">
                    Dislike
                </button>
            </div>
        </div>
    );
}

export default UserMatchCard;
