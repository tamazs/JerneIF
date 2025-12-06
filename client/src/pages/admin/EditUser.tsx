import {useEffect, useState} from "react";
import useJerneCrud from "../../hooks/useJerneCrud.ts";
import type {UpdateUserRequestDto, UserDto} from "../../generated-ts-client.ts";
import {useParams} from "react-router";
import {useAtom} from "jotai";
import {loggedInUserAtom, usersAtom} from "../../atoms/jerneAtom.ts";

export default function EditUser() {
    const jerneCrud = useJerneCrud();
    const [loggedInUser] = useAtom(loggedInUserAtom);
    const params = useParams();

    const [user, setUser] = useState<UserDto | null>(null);
    const [editUserForm, setEditUserForm] = useState<UpdateUserRequestDto | null>(null);

    // Load user
    useEffect(() => {
        if (!params.userId) return;

        jerneCrud.getUserById(params.userId).then((res) => {
            if (res) {
                setUser(res);
                setEditUserForm({
                    userId: res.userId,
                    fullName: res.fullName,
                    phoneNumber: res.phoneNumber,
                    email: res.email,
                    isActive: res.isActive,
                    role: res.role,
                    currentPassword: "",
                    newPassword: "",
                });
            }
        });
    }, [params.userId]);

    if (!editUserForm) {
        return (
            <div className="h-screen flex items-center justify-center">
                <span className="loading loading-spinner loading-lg"></span>
            </div>
        );
    }

    const isPasswordChange = editUserForm.currentPassword!.trim() === "";

    const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        jerneCrud.editUser(editUserForm);
    };

    return (
        <div className="h-screen flex items-center justify-center">
            <div className="w-full max-w-sm p-8 rounded-xl shadow-lg">
                <h1 className="text-2xl font-bold text-center mb-6">
                    Edit User
                </h1>

                <form onSubmit={handleSubmit} className="space-y-4">

                    <div>
                        <label className="label">
                            <span className="label-text">Full Name</span>
                        </label>
                        <input className="input validator" type="text" required placeholder="Your Name" value={editUserForm.fullName} onChange={(e) => setEditUserForm({ ...editUserForm, fullName: e.target.value })} />
                        <div className="validator-hint">Enter your name</div>
                    </div>

                    <div>
                        <label className="label">
                            <span className="label-text">Phone Number</span>
                        </label>
                        <input type="tel" className="input validator tabular-nums" required pattern="[0-9]*" minLength={8}
                               maxLength={8} title="Must be 8 digits" placeholder="12345678" value={editUserForm.phoneNumber} onChange={(e) => setEditUserForm({ ...editUserForm, phoneNumber: e.target.value })} />
                        <div className="validator-hint">Must be 8 characters</div>
                    </div>

                    <div>
                        <label className="label">
                            <span className="label-text">Email</span>
                        </label>
                        <input className="input validator" type="email" required placeholder="mail@site.com" value={editUserForm.email} onChange={(e) => setEditUserForm({ ...editUserForm, email: e.target.value })} />
                        <div className="validator-hint">Enter valid email address</div>
                    </div>

                    {loggedInUser?.role === "Admin" ?
                        <div>
                            <label className="label">
                                <input type="checkbox" checked={editUserForm.isActive ?? false} disabled={user!.isActive} className="checkbox" onChange={(e) => setEditUserForm({ ...editUserForm, isActive: e.target.checked })} />
                                Activate user
                            </label>
                        </div>
                        : null
                    }

                    {loggedInUser?.role === "Admin" ?
                        <div>
                            <label className="label">
                                <span className="label-text">Role</span>
                            </label>
                            <select value={editUserForm.role} className="select" onChange={(e) => setEditUserForm({ ...editUserForm, role: e.target.value })}>
                                <option disabled={true}>Pick a role</option>
                                <option value="Player">Player</option>
                                <option value="Admin">Admin</option>
                            </select>
                        </div>
                        : null
                    }

                    <div>
                        <label className="label">
                            <span className="label-text">Current Password</span>
                        </label>
                        <input type="password" className="input validator" placeholder="Password" minLength={8}
                               pattern="(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{8,}"
                               title="Must be more than 8 characters, including number, lowercase letter, uppercase letter"
                               onChange={(e) => setEditUserForm({ ...editUserForm, currentPassword: e.target.value })} />
                        <p className="validator-hint">
                            Must be more than 8 characters, including
                            <br/>At least one number
                            <br/>At least one lowercase letter
                            <br/>At least one uppercase letter
                        </p>
                    </div>

                    <div>
                        <label className="label">
                            <span className="label-text">New Password</span>
                        </label>
                        <input type="password" disabled={isPasswordChange} required={!isPasswordChange} className="input validator" placeholder="Password" minLength={8}
                               pattern="(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{8,}"
                               title="Must be more than 8 characters, including number, lowercase letter, uppercase letter"
                               onChange={(e) => setEditUserForm({ ...editUserForm, newPassword: e.target.value })} />
                        <p className="validator-hint">
                            Must be more than 8 characters, including
                            <br/>At least one number
                            <br/>At least one lowercase letter
                            <br/>At least one uppercase letter
                        </p>
                    </div>

                    <button
                        type="submit"
                        className="btn btn-secondary text-primary w-full rounded-lg"
                    >
                        Edit User
                    </button>
                </form>
            </div>
        </div>
    );
}
