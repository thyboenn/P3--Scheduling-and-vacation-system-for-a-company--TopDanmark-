<script lang="ts">
  import { Button, Card, TextField } from "attractions";
  import { push } from "svelte-spa-router";
  import topmarkLogo from "../assets/topmark.png";
  import type { components } from "../generated/schema";
  import { handleRegister } from "../lib/auth";
  import { redirectIfAuthenticated, type Route } from "../lib/routes";

  redirectIfAuthenticated();

  let loginLink: Route = "/login";

  let createdAssociatePromise: Promise<
    components["schemas"]["AuthResponseDTO"]
  > | null = null;

  let email = "";
  let password = "";
  let rePassword = "";
  let name = "";

  let hasErrors = false;

  async function tryRegister() {
    if (password === rePassword && (await handleRegister(name, email, password))) {
      redirectIfAuthenticated();
    } else {
      hasErrors = true;
    }
  }
</script>

<div class="register-container">
  <img src={topmarkLogo} alt="Topmark Logo" />
  <h1>Register</h1>
  <Card class="spaced">
    <TextField autofocus outline bind:value={name} label="Name" withItem />
    <TextField
      type="email"
      outline
      class="text-field"
      bind:value={email}
      label="Email"
      withItem
    />
    <TextField
      outline
      class="text-field"
      type="password"
      bind:value={password}
      label="Password"
      withItem
    />
    <TextField
      outline
      class="text-field"
      type="password"
      bind:value={rePassword}
      label="Repeat password"
      withItem
    />
    {#if hasErrors}
      <p class="danger">Fejl under login. Prøv igen</p>
    {/if}
  </Card>
  <Button on:click={tryRegister} style="margin-top: 20px" filled>
    Create account
  </Button>
  <Button style="margin-top: 20px" outline on:click={() => push(loginLink)}>
    Back
  </Button>

  {#if createdAssociatePromise != null}
    {#await createdAssociatePromise}
      <div>synkronisering</div>
    {:then associate}
      <pre>{JSON.stringify(associate)}</pre>
    {/await}
  {/if}
</div>

<style>
  .register-container {
    display: flex;
    margin: 2cm;
    flex-direction: column;
    justify-content: center;
    align-items: center;
  }
</style>
