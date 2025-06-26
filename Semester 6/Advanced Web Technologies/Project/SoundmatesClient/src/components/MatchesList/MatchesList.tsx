import { useCallback, useEffect, useRef, useState } from "react";
import { useNavigate } from "react-router";
import { useUtils } from "../../utils/UtilsContextType";
import type { OtherUserProfileDto } from "../../types/OtherUserProfileDto";

function MatchesList() {
    const navigate = useNavigate();

    const { getAccessToken, refreshAccessToken } = useUtils();

    const [loading, setLoading] = useState(true);
    const [matches, setMatches] = useState<OtherUserProfileDto[]>([]);
    const offset = useRef(0);

    const handleBack = () => {
        navigate("/");
    };

    const loadMatches = useCallback(async () => {
        const params = new URLSearchParams();
        params.append("limit", "20");
        params.append("offset", `${offset.current}`);

        const url = `http://localhost:5000/matching/matches?${params}`;

        const request = new Request(url, {
            headers: {
                "Access-Token": getAccessToken(),
            },
        });

        let response = await fetch(request);

        if (response.status === 401) {
            const refreshed = await refreshAccessToken();
            if (!refreshed) {
                return false;
            }
            response = await fetch(request);
        }

        if (!response.ok) {
            return false;
        }

        const data = (await response.json()) as OtherUserProfileDto[];

        setMatches(data);
        offset.current += 20;
        return true;
    }, [getAccessToken, refreshAccessToken]);

    useEffect(() => {
        const initData = async () => {
            const loaded = await loadMatches();

            if (!loaded) {
                return;
            }

            setLoading(false);
        };

        initData();
    }, [loadMatches]);

    return (
        <div className="m-3">
            <button
                onClick={handleBack}
                className="btn btn-dark mb-3">
                Back
            </button>
            {loading && <div>Loading...</div>}
            {!loading && (
                <div>
                    <h3>Matches list</h3>
                    {matches.length === 0 ? (
                        <p>No matches found.</p>
                    ) : (
                        <ul className="list-group">
                            {matches.map((user: OtherUserProfileDto) => (
                                <li
                                    key={user.id}
                                    className="list-group-item">
                                    <div className="d-flex justify-content-between">
                                        <div>
                                            <strong>
                                                {user.name}, {new Date().getFullYear() - user.birthYear}
                                            </strong>
                                            — {user.city}, {user.country}
                                        </div>
                                        <div>
                                            <button className="btn btn-primary ms-3">View profile</button>
                                            <button className="btn btn-success ms-3">Message</button>
                                        </div>
                                    </div>
                                </li>
                            ))}
                        </ul>
                    )}
                </div>
            )}
        </div>
    );
}

export default MatchesList;
