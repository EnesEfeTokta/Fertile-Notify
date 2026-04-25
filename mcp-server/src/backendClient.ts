import axios from "axios";
import { API_BASE_URL, API_KEY } from "./config.js";

export const backendClient = axios.create({
  baseURL: API_BASE_URL,
});

if (API_KEY) {
  backendClient.defaults.headers.common["FN-Api-Key"] = API_KEY;
}
