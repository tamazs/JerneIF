import { useState } from "react";
import type {LoginRequestDto} from "../generated-ts-client.ts";
import useJerneCrud from "../hooks/useJerneCrud.ts";

export default function LoginPage() {
    const jerneCrud = useJerneCrud();
    const [loginForm, setLoginForm] = useState<LoginRequestDto>({
        email: "",
        password: ""
    })

    const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        jerneCrud.loginUser(loginForm);
    };

    return (
        <div className="min-h-screen flex items-center justify-center">
            <div className="w-full max-w-sm p-8 rounded-xl shadow-lg">
                <h1 className="text-2xl font-bold text-center mb-6">
                    Login
                </h1>

                <form onSubmit={handleSubmit} className="space-y-4">

                    {/* Email */}
                    <div>
                        <label className="label">
                            <span className="label-text">Email</span>
                        </label>
                        <input className="input validator" type="email" required placeholder="mail@site.com" onChange={(e) => setLoginForm({ ...loginForm, email: e.target.value })} />
                        <div className="validator-hint">Enter valid email address</div>
                    </div>

                    {/* Password */}
                    <div>
                        <label className="label">
                            <span className="label-text">Password</span>
                        </label>
                        <input type="password" className="input validator" required placeholder="Password" minLength={8}
                               pattern="(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{8,}"
                               title="Must be more than 8 characters, including number, lowercase letter, uppercase letter"
                        onChange={(e) => setLoginForm({ ...loginForm, password: e.target.value })} />
                        <p className="validator-hint">
                            Must be more than 8 characters, including
                            <br/>At least one number
                            <br/>At least one lowercase letter
                            <br/>At least one uppercase letter
                        </p>
                    </div>

                    {/* Submit Button */}
                    <button
                        type="submit"
                        className="btn w-full rounded-lg"
                    >
                        Log In
                    </button>
                </form>
            </div>
        </div>
    );
}
